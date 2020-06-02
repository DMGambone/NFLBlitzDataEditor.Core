using System;

namespace MidwayGamesFS
{
    /// <summary>
    /// Contains the information stored in the file system header
    /// </summary>
    public class FileSystemHeader
    {
        /// <summary>
        /// THe file system checksum
        /// </summary>
        public uint Checksum { get; set; }

        /// <summary>
        /// The location of the first partition
        /// </summary>
        public int FirstPartitionAddress { get; set; }
    }
}