using System;

namespace NFLBlitzDataEditor.Core.FileSystem
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
        public uint Size { get; set; }

        /// <summary>
        /// The file timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The location of the file on the physical drive
        /// </summary>
        public uint Address { get; set; }
    }
}