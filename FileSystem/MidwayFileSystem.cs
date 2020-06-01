using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using NFLBlitzDataEditor.Core.Models;
using NFLBlitzDataEditor.Core.Extensions;

namespace NFLBlitzDataEditor.Core.FileSystem
{
    /// <summary>
    /// Provides functionality to reading and writing from a Midway file system
    /// </summary>
    public class MidwayFileSystem
        : IMidwayFileSystem
    {
        /// <summary>
        /// The size of a single sector in the hard drive
        /// </summary>
        protected readonly uint SectorSize = 0x200;  //512 byte sectors

        /// <summary>
        /// The size of a single block in the file system
        /// </summary>
        protected readonly uint BlockSize = 0x1000;  //4k blocks

        /// <summary>
        /// The size of a partition entry table
        /// </summary>
        protected readonly uint PartitionFATSize = 0xff0; //4080 byte sectors

        /// <summary>
        /// The file system header information
        /// </summary>
        protected FileSystemHeader Header;

        /// <summary>
        /// The address to resolve all address from
        /// </summary>
        protected uint BaseAddress { get; private set; }

        /// <summary>
        /// The complete list of file system partitions
        /// </summary>
        private IEnumerable<FileSystemPartition> _partitions = new FileSystemPartition[0];

        private IEnumerable<string> _extensionsWithCRC = new string[] { ".WMS", ".BNK", ".EXE" };

        /// <summary>
        /// A mapping of file names to their entry information
        /// </summary>
        /// <typeparam name="string">The name of the file</typeparam>
        /// <typeparam name="FileSystemEntry">The file system entry information</typeparam>
        private IDictionary<string, FileAllocationTableEntry> _fileAllocationTable = new Dictionary<string, FileAllocationTableEntry>();

        /// <summary>
        /// The read/write stream used to access the file system
        /// </summary>
        private readonly Stream _fileSystemStream;

        /// <summary>
        /// Initializes the file system
        /// </summary>
        public MidwayFileSystem(Stream fileSystemStream, uint sectorSize, uint blockSize, uint partitionEntryTableSize)
        {
            _fileSystemStream = fileSystemStream;
            SectorSize = sectorSize;
            BlockSize = blockSize;
            PartitionFATSize = partitionEntryTableSize;

            //Load the file system header information
            LoadFileSystemHeader();

            //Load the partition and file allocation table
            LoadPartitions();
        }

        /// <summary>
        /// Resolves a sector value into a location within the file system
        /// </summary>
        /// <param name="sector">The sector value to resolve</param>
        protected virtual uint ResolveSectorAddress(uint sector)
        {
            return sector * SectorSize;
        }

        /// <summary>
        /// Resolves a block value into a location within the file system
        /// </summary>
        /// <param name="block">The block value to resolve</param>
        protected virtual uint ResolveBlockAddress(uint block)
        {
            return BaseAddress + (block * BlockSize);
        }

        /// <summary>
        /// Reads a single file allocation table entry from a BinaryReader
        /// </summary>
        /// <param name="reader">The reader used to read the entry from</param>
        /// <returns>An instance of <see cref="FileAllocationTableEntry" /></returns>
        protected virtual FileAllocationTableEntry ReadFATEntry(BinaryReader reader)
        {
            //Read in the filename, which is null-terminated up to 12 characters
            string fileName = reader.ReadAsString(12, true);
            if (string.IsNullOrEmpty(fileName))
                return null;

            //Read the file size
            return new FileAllocationTableEntry()
            {
                Name = fileName,
                Size = reader.ReadUInt32() * 4,
                Timestamp = reader.ReadTimestamp(),
                Address = reader.ReadUInt32()
            };
        }

        /// <summary>
        /// Reads all the entries in a single partition
        /// </summary>
        /// <param name="reader">The reader used to read the partition entries from</param>
        /// <returns>An collection of <see cref="FileAllocationTableEntry" /></returns>
        protected virtual ICollection<FileAllocationTableEntry> ReadPartitionFAT(BinaryReader reader)
        {
            byte[] entriesTable = reader.ReadBytes((int)PartitionFATSize);
            IList<FileAllocationTableEntry> entries = new List<FileAllocationTableEntry>();

            BinaryReader headerTableReader = new BinaryReader(new MemoryStream(entriesTable));
            while (headerTableReader.PeekChar() != -1)
            {
                FileAllocationTableEntry entry = ReadFATEntry(headerTableReader);
                if (entry == null)
                    break;

                //Resolve the entry's actual file location
                entry.Address = ResolveBlockAddress(entry.Address);

                entries.Add(entry);
            }

            return entries.ToArray();
        }

        /// <summary>
        /// Reads a specific sector
        /// </summary>
        /// <param name="sectorIndex">The sector index to read</param>
        /// <returns>A collection of bytes from that sector</returns>
        protected byte[] ReadSector(uint sectorIndex)
        {
            _fileSystemStream.Seek(ResolveSectorAddress(sectorIndex), SeekOrigin.Begin);
            BinaryReader reader = new BinaryReader(_fileSystemStream);

            return reader.ReadBytes((int)SectorSize);
        }

        /// <summary>
        /// Returns the file system header
        /// </summary>
        protected virtual void LoadFileSystemHeader()
        {
            //Read in the file system header, located in the 3rd sector
            byte[] sectorData = ReadSector(3);

            using (BinaryReader reader = new BinaryReader(new MemoryStream(sectorData)))
            {
                Header = new FileSystemHeader();

                Header.Checksum = reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();

                //Get the location of the first partion.  The value is a sector index
                Header.FirstPartitionAddress = ResolveSectorAddress(reader.ReadUInt32() + 1);
            }
        }

        /// <summary>
        /// Loads the file system information
        /// </summary>
        protected virtual void LoadPartitions()
        {
            //TODO: Load in the complete drive header information

            //Set the base address for resolving addresses
            //Why is the start of the addressing 1,536 before the first partition? 
            uint partitionAddress = Header.FirstPartitionAddress;
            BaseAddress = partitionAddress - (3 * BlockSize);

            //Go to the starting partition location.
            BinaryReader reader = new BinaryReader(_fileSystemStream);
            reader.BaseStream.Seek(partitionAddress, SeekOrigin.Begin);

            //Keep reading each partition
            IList<FileSystemPartition> partitions = new List<FileSystemPartition>();
            while (reader.PeekChar() != -1)
            {
                FileSystemPartition partition = new FileSystemPartition()
                {
                    Address = partitionAddress,
                    FileAllocationTable = ReadPartitionFAT(reader)
                };

                partitions.Add(partition);
                foreach (FileAllocationTableEntry entry in partition.FileAllocationTable)
                    _fileAllocationTable.Add(entry.Name, entry);

                reader.ReadAsUInt32Array(3);

                //Go to the next partition
                uint nextPartitionBlock = reader.ReadUInt32();
                if (nextPartitionBlock == 0)
                    break;

                //Go to the next starting partition location.
                partitionAddress = ResolveBlockAddress(nextPartitionBlock);
                reader.BaseStream.Seek(partitionAddress, SeekOrigin.Begin);
            }

            _partitions = partitions;
        }

        /// <inheritdocs />
        public IEnumerable<FileAllocationTableEntry> GetFiles()
        {
            return _fileAllocationTable.Values.ToArray();
        }

        /// <inheritdocs />
        public System.IO.Stream OpenRead(string fileName)
        {
            //Find the file entry
            if (!_fileAllocationTable.ContainsKey(fileName))
                throw new System.IO.FileNotFoundException($"{fileName} does not exist in this file system");

            FileAllocationTableEntry entry = _fileAllocationTable[fileName];

            BinaryReader reader = new BinaryReader(_fileSystemStream);
            _fileSystemStream.Seek(entry.Address, SeekOrigin.Begin);
            byte[] buffer = reader.ReadBytes((int)entry.Size);

            int offset = 0;
            int size = buffer.Length;

            if(_extensionsWithCRC.Contains(Path.GetExtension(entry.Name)))
            {
                offset += 4;
                size -= 4;
            }

            return new MemoryStream(buffer, offset, size);
        }

        /// <inheritdocs />
        public System.IO.Stream OpenWrite(string fileName, bool createNew)
        {
            throw new NotImplementedException("OpenWrite is not implemented yet");
        }


    }
}