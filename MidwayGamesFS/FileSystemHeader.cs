using System;

namespace MidwayGamesFS
{
    /// <summary>
    /// Contains the information stored in the file system header
    /// </summary>
    public class FileSystemHeader
    {
        /// <summary>
        /// The file system unique signature
        /// </summary>
        public uint Signature { get; set; }

        /// <summary>
        /// The location of the first file allocation table
        /// </summary>
        public int FirstFileAllocationTablePosition { get; set; }

        /// <summary>
        /// The location of the cluster table
        /// </summary>
        public int ClusterTablePosition { get; set; }

        /// <summary>
        /// The number of sectors in a cluster.  Sectors are 512 bytes long and there are typically 8 per cluster (4096 bytes)
        /// </summary>
        public int SectorsPerCluster { get; set; }
    }
}