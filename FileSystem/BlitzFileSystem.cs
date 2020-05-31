using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using NFLBlitzDataEditor.Core.Models;
using NFLBlitzDataEditor.Core.Extensions;

namespace NFLBlitzDataEditor.Core.FileSystem
{
    /// <summary>
    /// Provides functionality to reading and writing from a Blitz file system
    /// </summary>
    public class BlitzFileSystem
        : MidwayFileSystem
    {
        /// <summary>
        /// Initializes the file system
        /// </summary>
        /// <param name="fileSystemStream">A read/write stream used to access the file system</param>
        public BlitzFileSystem(Stream fileSystemStream)
            : base(fileSystemStream, 0x200, 0x1000, 0xff0)
        {
        }
   }
}