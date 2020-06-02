using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

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
        protected readonly int SectorSize = 0x200;  //512 byte sectors

        /// <summary>
        /// The size of a single block in the file system
        /// </summary>
        protected readonly int BlockSize = 0x1000;  //4k blocks

        /// <summary>
        /// The maximum size of a partition file allocation table
        /// </summary>
        protected readonly int PartitionFATSize = 0xff0; //4080 byte sectors

        /// <summary>
        /// The maximum number of entries in a partition's file allocation table
        /// </summary>
        protected readonly int MaximumFATEntries = 170;

        /// <summary>
        /// The file system header information
        /// </summary>
        protected FileSystemHeader Header;

        /// <summary>
        /// The position to resolve all address from
        /// </summary>
        protected int BaseAddress { get; private set; }

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
        public FileSystem(IDataBuffer data)
        {
            _fileSystemData = data;

            //Load the file system header information
            LoadFileSystemHeader();

            //Load the partition and file allocation table
            LoadPartitions();
        }

        /// <summary>
        /// Resolves a sector value into a location within the file system
        /// </summary>
        /// <param name="sector">The sector value to resolve</param>
        protected virtual int ResolveSectorPosition(int sector)
        {
            return sector * SectorSize;
        }

        /// <summary>
        /// Resolves a block value into a location within the file system
        /// </summary>
        /// <param name="block">The block value to resolve</param>
        protected virtual int ResolveBlockPosition(int block)
        {
            return BaseAddress + (block * BlockSize);
        }

        /// <summary>
        /// Reads the next value as a Date/Time value 
        /// </summary>
        /// <param name="reader">The reader to read the int from</param>
        protected DateTime ConvertToTimestamp(int data)
        {
            //A timestamp is stored in 32 bits, with the time in the first 16 bits and date in the second 16 bits.
            ushort time = (ushort)(data & 0xff);
            ushort date = (ushort)((data >> 16) & 0xff);

            //Time: hours, minutes, seconds are stored as 5 bits, 6 bits, 5 bits (respectively)
            int hour = (time >> 11) & 0x1f;
            int minute = (time >> 5) & 0x3f;
            int second = (time & 0x1f) * 2;

            //Date: year (since 1980), month, day are stored as 7 bits, 4 bits, 5 bits (respectively)
            int year = (date >> 9) & 0x5f;
            int month = (date >> 5) & 0x0f;
            int day = date & 0x1f;

            if (year == 0 || month == 0 || day == 0)
                return DateTime.MinValue;

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
            int position = reader.ReadInt32();
            int checksum = 0;
            if (_extensionsWithCRC.Contains(Path.GetExtension(fileName)))
                checksum = GetFileChecksum(ResolveBlockPosition(position));

            return new FileAllocationTableEntry()
            {
                Name = fileName,
                Size = size,
                Timestamp = timestamp,
                Position = position,
                Checksum = checksum
            };
        }

        /// <summary>
        /// Reads all the entries in a single partition
        /// </summary>
        /// <param name="reader">The reader used to read the partition entries from</param>
        /// <returns>An collection of <see cref="FileAllocationTableEntry" /></returns>
        protected virtual ICollection<FileAllocationTableEntry> ReadPartitionFAT(BinaryReader reader)
        {
            IList<FileAllocationTableEntry> entries = new List<FileAllocationTableEntry>();
            while (entries.Count < 170)
            {
                FileAllocationTableEntry entry = ReadFATEntry(reader);
                if (entry == null)
                    break;

                //Resolve the entry's actual file location
                entry.Position = ResolveBlockPosition(entry.Position);

                entries.Add(entry);
            }

            return entries.ToArray();
        }

        /// <summary>
        /// Reads a specific sector
        /// </summary>
        /// <param name="sectorIndex">The sector index to read</param>
        /// <returns>A collection of bytes from that sector</returns>
        protected virtual BinaryReader ReadSector(int sectorIndex)
        {
            return _fileSystemData.GetReader(ResolveSectorPosition(sectorIndex), SectorSize);
        }

        /// <summary>
        /// Returns the file system header
        /// </summary>
        protected virtual void LoadFileSystemHeader()
        {
            //Read in the file system header, located in the 3rd sector
            using (BinaryReader reader = ReadSector(3))
            {
                Header = new FileSystemHeader();

                Header.Checksum = reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();

                //Get the location of the first partion.  The value is a sector index
                Header.FirstPartitionPosition = ResolveSectorPosition(reader.ReadInt32() + 1);
            }
        }

        /// <summary>
        /// Returns the checksum of a file
        /// </summary>
        /// <param name="position">The location of the file where the checksum would be</param>
        /// <returns>The file's CRC checksum.  If it does not use a checksum, 0 is returned</returns>
        protected int GetFileChecksum(int position)
        {
            BinaryReader reader = _fileSystemData.GetReader(position, 4);
            return reader.ReadInt32();
        }

        /// <summary>
        /// Loads the file system information
        /// </summary>
        protected virtual void LoadPartitions()
        {
            //TODO: Load in the complete drive header information

            //Set the base position for resolving positions
            //Why is the start of the positions 1,536 before the first partition? 
            int partitionPosition = Header.FirstPartitionPosition;
            BaseAddress = partitionPosition - (3 * BlockSize);

            //Keep reading each partition
            IList<FileSystemPartition> partitions = new List<FileSystemPartition>();
            while (partitionPosition != 0)
            {
                //Read the first block of the partition, which contains the allocation table as well as pointer to the next partition
                BinaryReader reader = _fileSystemData.GetReader(partitionPosition, BlockSize);

                FileSystemPartition partition = new FileSystemPartition()
                {
                    Position = partitionPosition,
                    FileAllocationTable = ReadPartitionFAT(reader)
                };

                partitions.Add(partition);
                foreach (FileAllocationTableEntry entry in partition.FileAllocationTable)
                    _fileAllocationTable.Add(entry.Name, entry);

                //Read the next 3 words, but those aren't used
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();

                //Go to the next partition
                int nextPartitionBlock = reader.ReadInt32();
                if (nextPartitionBlock == 0)
                    break;

                //Go to the next starting partition location.
                partitionPosition = ResolveBlockPosition(nextPartitionBlock);
            }

            _partitions = partitions;
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
        public System.IO.Stream OpenRead(string fileName)
        {
            //Find the file entry
            FileAllocationTableEntry entry = GetFileInfo(fileName);
            int offset = entry.Position;
            int size = entry.Size;

            if (entry.Checksum != 0)
            {
                offset += 4;
                size -= 4;
            }

            //Read the complete file from the file system and return a stream
            return new MemoryStream(_fileSystemData.Get(offset, size));
        }

        /// <inheritdocs />
        public System.IO.Stream OpenWrite(string fileName, bool createNew)
        {
            throw new NotImplementedException("OpenWrite is not implemented yet");
        }


    }
}