using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace MidwayGamesFS
{
    /// <summary>
    /// Provides functionality to reading and writing from a Midway file system
    /// </summary>
    public class FileSystem
        : IFileSystem
    {
        /// <summary>
        /// The size of a single sector in the hard drive
        /// </summary>
        public readonly int SectorSize = 0x200;  //512 byte sectors

        /// <summary>
        /// The size of a single block in the file system
        /// </summary>
        public readonly int ClusterSize = 0x1000;  //4k blocks

        /// <summary>
        /// The maximum size of a partition file allocation table
        /// </summary>
        protected readonly int PartitionFATSize = 0xff0; //4080 byte table

        /// <summary>
        /// The maximum number of entries in a partition's file allocation table
        /// </summary>
        protected readonly int MaximumFATEntries = 170;

        /// <summary>
        /// The file system header information
        /// </summary>
        protected FileSystemHeader Header;

        /// <summary>
        /// The virtual sector to physical sector lookup table
        /// </summary>
        private int[] _clusterTable = new int[0];

        /// <summary>
        /// The position to resolve all clusters from
        /// </summary>
        protected int BaseClusterOffset { get; private set; }

        /// <summary>
        /// The complete list of file system partitions
        /// </summary>
        private IEnumerable<FileSystemPartition> _partitions = new FileSystemPartition[0];

        /// <summary>
        /// List of file extensions that have CRC checksums
        /// </summary>
        /// <value></value>
        private IEnumerable<string> _extensionsWithCRC = new string[] { ".WMS", ".BNK", ".EXE" };

        /// <summary>
        /// A mapping of file names to their entry information
        /// </summary>
        /// <typeparam name="string">The name of the file</typeparam>
        /// <typeparam name="FileSystemEntry">The file system entry information</typeparam>
        private IDictionary<string, FileAllocationTableEntry> _fileAllocationTable = new Dictionary<string, FileAllocationTableEntry>();

        /// <summary>
        /// The raw data of the file system
        /// </summary>
        private readonly IDataBuffer _fileSystemData;

        /// <summary>
        /// Initializes the filesystem
        /// </summary>
        /// <param name="data">The raw file system data</param>
        /// <param name="fileSystemSectorIndex">The index of the file system information</param>
        public FileSystem(IDataBuffer data, int fileSystemSectorIndex)
        {
            _fileSystemData = data;

            //Load the file system information
            LoadFileSystemInfo(fileSystemSectorIndex);

            ClusterSize = Header.SectorsPerCluster * SectorSize;
            BaseClusterOffset = Header.FirstFileAllocationTablePosition - (3 * ClusterSize);

            //Load the partition and file allocation table
            LoadFileAllocationTable();
        }

        /// <summary>
        /// Resolves a sector value into a location within the file system
        /// </summary>
        /// <param name="sector">The sector value to resolve</param>
        protected virtual int ResolveSectorPosition(int sector)
        {
            return (sector + 1) * SectorSize;
        }

        /// <summary>
        /// Resolves the physical location of a cluster based on it's id
        /// </summary>
        /// <param name="clusterId">The id of the cluster to resolve</param>
        protected virtual int ResolveClusterPosition(int clusterId)
        {
            return BaseClusterOffset + (clusterId * ClusterSize);
        }

        /// <summary>
        /// Reads the next value as a Date/Time value 
        /// </summary>
        /// <param name="reader">The reader to read the int from</param>
        protected DateTime ConvertToTimestamp(int data)
        {
            if (data == 0)
                return DateTime.MinValue;

            //A timestamp is stored in 32 bits, with the time in the first 16 bits and date in the second 16 bits.
            ushort time = (ushort)(data & 0xffff);
            ushort date = (ushort)((data >> 16) & 0xffff);

            //Time: hours, minutes, seconds are stored as 5 bits, 6 bits, 5 bits (respectively)
            int hour = (time >> 11) & 0x1f;
            int minute = (time >> 6) & 0x3f;
            int second = (time & 0x1f) * 2;

            //Date: year (since 1980), month, day are stored as 7 bits, 4 bits, 5 bits (respectively)
            int year = (date >> 9) & 0x7f;
            int month = (date >> 5) & 0x0f;
            int day = date & 0x1f;

            return new DateTime(1980 + year, month, day, hour, minute, second);
        }

        /// <summary>
        /// Swaps the bytes in a 32-bit word
        /// </summary>
        /// <param name="word">The 32-bit word to swap the values for</param>
        /// <returns>A value of <paramref name="word" /> with the endianness reversed</returns>
        protected byte[] SwapBytes(byte[] word)
        {
            return new byte[4] { word[3], word[2], word[1], word[0] };
        }

        /// <summary>
        /// Converts a collection of bytes into a string
        /// </summary>
        /// <param name="data">The data to convert</param>
        protected string ConvertToString(byte[] data)
        {
            //Loop through the string and flip the bytes in each word
            for (int i = 0; i < data.Length; i += 4)
            {
                byte[] word = new byte[4] { data[i], data[i + 1], data[i + 2], data[i + 3] };
                word = SwapBytes(word);
                data[i] = word[0];
                data[i + 1] = word[1];
                data[i + 2] = word[2];
                data[i + 3] = word[3];
            }

            string value = System.Text.Encoding.ASCII.GetString(data);
            return value.TrimEnd((char)0);
        }

        /// <summary>
        /// Returns the set of clusters from the cluster table
        /// </summary>
        /// <param name="firstClusterIndex">The index of the first cluster in the cluster table</param>
        /// <param name="numClusters">The total number of clusters to return</param>
        /// <returns>A list of cluster Ids</return>
        protected virtual int[] GetClusterTableEntries(int firstClusterIndex, int numClusters)
        {
            int[] clusters = _clusterTable.Skip(firstClusterIndex - 4).Take(numClusters).ToArray();
            clusters[0] = firstClusterIndex;

            return clusters;
        }

        /// <summary>
        /// Reads a single file allocation table entry from a BinaryReader
        /// </summary>
        /// <param name="reader">The reader used to read the entry from</param>
        /// <returns>An instance of <see cref="FileAllocationTableEntry" /></returns>
        protected virtual FileAllocationTableEntry ReadFATEntry(BinaryReader reader)
        {
            //Read in the filename, which is null-terminated up to 12 characters
            string fileName = ConvertToString(reader.ReadBytes(12));
            if (string.IsNullOrEmpty(fileName))
                return null;

            //Read the file size
            int size = reader.ReadInt32() * 4;
            DateTime timestamp = ConvertToTimestamp(reader.ReadInt32());
            int clusterIndex = reader.ReadInt32();

            //Get the number of clusters a single file would occupy (remember to round up)
            int numClusters = (int)Math.Ceiling((double)size / ClusterSize);

            return new FileAllocationTableEntry()
            {
                Name = fileName,
                Size = size,
                Timestamp = timestamp,
                Clusters = GetClusterTableEntries(clusterIndex, numClusters)
            };
        }

        /// <summary>
        /// Reads all the entries in a single partition
        /// </summary>
        /// <param name="reader">The reader used to read the partition entries from</param>
        /// <returns>An collection of <see cref="FileAllocationTableEntry" /></returns>
        protected virtual ICollection<FileAllocationTableEntry> ReadFileAllocationTable(BinaryReader reader)
        {
            IList<FileAllocationTableEntry> entries = new List<FileAllocationTableEntry>();
            while (entries.Count < MaximumFATEntries)
            {
                FileAllocationTableEntry entry = ReadFATEntry(reader);
                if (entry == null)
                    break;

                entries.Add(entry);
            }

            return entries.ToArray();
        }

        /// <summary>
        /// Returns the virtual sector table starting at
        /// </summary>
        /// <param name="offset">The offset where the table begins</param>
        /// <param name="maxSize">The maximum number of entries in the table</param>
        protected virtual int[] ReadVirtualSectorTable(int offset, int maxSize)
        {
            int[] sectorTable = new int[maxSize];

            using (BinaryReader reader = _fileSystemData.GetReader(offset, maxSize * 2))
            {
                for (int index = 0; index < maxSize; index++)
                {
                    sectorTable[index] = reader.ReadInt16();
                }
            }

            return sectorTable;
        }

        /// <summary>
        /// Returns the file system header
        /// </summary>
        /// <param name="fileSystemSectorIndex">The index of the file system information</param>
        protected virtual void LoadFileSystemInfo(int fileSystemSectorIndex)
        {
            //Read in the file system information
            using (BinaryReader reader = new BinaryReader(ReadPhysicalSector(fileSystemSectorIndex)))
            {
                Header = new FileSystemHeader();

                Header.Signature = reader.ReadUInt32();

                Header.SectorsPerCluster = reader.ReadInt32();

                Header.ClusterTablePosition = ResolveSectorPosition(reader.ReadInt32());

                Header.FirstFileAllocationTablePosition = ResolveSectorPosition(reader.ReadInt32());
            }

            _clusterTable = ReadVirtualSectorTable(Header.ClusterTablePosition, (Header.FirstFileAllocationTablePosition - Header.ClusterTablePosition) / 2);
        }

        /// <summary>
        /// Returns the checksum of a file
        /// </summary>
        /// <param name="position">The location of the file where the checksum would be</param>
        /// <returns>The file's CRC checksum.  If it does not use a checksum, 0 is returned</returns>
        protected uint GetFileChecksum(int position)
        {
            BinaryReader reader = _fileSystemData.GetReader(position, 4);
            return reader.ReadUInt32();
        }

        /// <summary>
        /// Loads the complete file allocation table 
        /// </summary>
        protected virtual void LoadFileAllocationTable()
        {
            //Set the base position for resolving positions
            //Why is the start of the positions 12,288 before the first partition? 
            int partitionPosition = Header.FirstFileAllocationTablePosition;
            int partitionBaseOffset = partitionPosition - (3 * ClusterSize);

            //Keep reading each partition
            IList<FileSystemPartition> partitions = new List<FileSystemPartition>();
            while (partitionPosition != 0)
            {
                //Read the first block of the partition, which contains the allocation table as well as pointer to the next partition
                BinaryReader reader = _fileSystemData.GetReader(partitionPosition, ClusterSize);

                FileSystemPartition partition = new FileSystemPartition()
                {
                    Position = partitionPosition,
                    FileAllocationTable = ReadFileAllocationTable(reader)
                };

                partitions.Add(partition);
                foreach (FileAllocationTableEntry entry in partition.FileAllocationTable)
                    _fileAllocationTable.Add(entry.Name, entry);

                //Read the next 3 words, but those aren't used
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();

                //The last word is the address to the next partition.  If the value is not 0 (end of partitions), 
                // then go to the next partition
                int nextPartitionBlock = reader.ReadInt32();
                if (nextPartitionBlock == 0)
                    break;

                //Go to the next starting partition location.
                partitionPosition = BaseClusterOffset + (nextPartitionBlock * ClusterSize);
            }

            //TODO: The cluster after the last partition contains the root directory


            _partitions = partitions;
        }

        /// <inheritdocs />
        public Stream ReadPhysicalSector(int sectorIndex)
        {
            byte[] sectorData = _fileSystemData.Get(ResolveSectorPosition(sectorIndex), SectorSize);
            return new MemoryStream(sectorData);
        }

        /// <inheritdocs />
        public IEnumerable<FileAllocationTableEntry> GetFiles()
        {
            return _fileAllocationTable.Values.ToArray();
        }

        /// <inheritdocs />
        public FileAllocationTableEntry GetFileInfo(string fileName)
        {
            if (!_fileAllocationTable.ContainsKey(fileName))
                throw new System.IO.FileNotFoundException($"{fileName} does not exist in this file system");

            return _fileAllocationTable[fileName];
        }

        /// <inheritdocs />
        public void SaveFileInfo(FileAllocationTableEntry entry)
        {

        }

        /// <inheritdocs />
        public Stream OpenRead(string fileName)
        {
            //Find the file entry
            FileAllocationTableEntry entry = GetFileInfo(fileName);

            MemoryStream stream = new MemoryStream();
            int remainingSize = entry.Size;
            bool firstCluster = true;
            foreach (int clusterId in entry.Clusters)
            {
                int position = ResolveClusterPosition(clusterId);
                int size = remainingSize < ClusterSize ? remainingSize : ClusterSize;
                if(firstCluster && _extensionsWithCRC.Contains(Path.GetExtension(fileName)))
                {
                    position += 4;
                    size -= 4;
                }
                
                byte[] buffer = _fileSystemData.Get(position, size);
                stream.Write(buffer);
                remainingSize -= buffer.Length;
                firstCluster = false;
            }

            stream.Position = 0;
            return stream;
        }

        /// <inheritdocs />
        public Stream OpenWrite(string fileName, bool createNew)
        {
            throw new NotImplementedException("OpenWrite is not implemented yet");
        }


    }
}