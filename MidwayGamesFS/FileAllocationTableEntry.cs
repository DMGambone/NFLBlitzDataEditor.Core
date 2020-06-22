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
        /// The clusters that contain the file
        /// </summary>
        public int[] Clusters {get; set; } = new int[0];
    }
}