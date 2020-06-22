using System;
using System.Collections.Generic;
using MidwayGamesFS;

namespace NFLBlitzDataEditor.Core
{
    /// <summary>
    /// Provides functionality to reading and writing from a Blitz file system
    /// </summary>
    public class BlitzFileSystem
        : MidwayGamesFS.FileSystem
    {
        /// <summary>
        /// Initializes the filesystem
        /// </summary>
        /// <param name="data">The raw file system data</param>
        public BlitzFileSystem(IDataBuffer data)
            : base(data, 2)
        {
        }
   }
}