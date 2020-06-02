using System;

namespace MidwayGamesFS
{
    /// <summary>
    /// Represents a single entry in the file system
    /// </summary>
    public class FileAllocationTableEntry
    {
        /// <summary>
        /// The name of the file
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The size of the file
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// The file timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The location of the file on the physical drive
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// The file's checksum.  If the file does not have a checksum, this is 0
        /// </summary>
        public int Checksum {get; set; }
    }
}