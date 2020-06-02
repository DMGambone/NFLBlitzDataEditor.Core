using System.Collections.Generic;

namespace MidwayGamesFS
{
    /// <summary>
    /// Represents a partition with the file system, which contains up to 170 <see cref="FileAllocationTableEntry" /> records
    /// </summary>
    public class FileSystemPartition
    {
        /// <summary>
        /// The base address of the partition
        /// </summary>
        /// <value></value>
        public int Address { get; set; }

        /// <summary>
        /// The entries in the partition
        /// </summary>
        public ICollection<FileAllocationTableEntry> FileAllocationTable { get; set; } = new FileAllocationTableEntry[0];
    }
}