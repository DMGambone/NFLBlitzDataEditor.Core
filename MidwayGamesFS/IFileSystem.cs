using System.IO;
using System.Collections.Generic;

namespace MidwayGamesFS
{
    /// <summary>
    /// Interface for reading and writing from a Midway Games file system
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Gets the complete list of files in the file system
        /// </summary>
        /// <returns>A collection of <see cref="FileAllocationTableEntry" /> of all the files in the system.</returns>
        IEnumerable<FileAllocationTableEntry> GetFiles();

        /// <summary>
        /// Gets the information about a specific file
        /// </summary>
        /// <returns>An instance of <see cref="FileAllocationTableEntry" /> for a specific file.</returns>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when the file specified was not found</exception>
        FileAllocationTableEntry GetFileInfo(string fileName);

        /// <summary>
        /// Saves information about a specific file in the file system
        /// </summary>
        /// <param name="entry">An instance of <see cref="FileAllocationTableEntry" /> for a specific file.</param>
        void SaveFileInfo(FileAllocationTableEntry entry);

        /// <summary>
        /// Reads a specific sector
        /// </summary>
        /// <param name="sectorIndex">The sector index to read</param>
        /// <returns>A collection of bytes from that sector</returns>
        Stream ReadPhysicalSector(int sectorIndex);

        /// <summary>
        /// Returns a readable stream for a specific file
        /// </summary>
        /// <param name="fileName">The name of the file to open a stream to</param>
        /// <returns>An instance of a readable <see cref="Stream" /> to the file.  The stream will only contain the contents of the file.</returns>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when the file specified was not found</exception>
        Stream OpenRead(string fileName);

        /// <summary>
        /// Returns a writeable stream for a specific file
        /// </summary>
        /// <param name="fileName">The name of the file to open a stream to</param>
        /// <param name="createNew">Indicates if the file is a new file and should be added to the last partition
        /// <returns>An instance of a writeable <see cref="Stream" /> to the file.  <paramref name="createNew"> is false, the stream will only allow to be written to the block space occupied by an existing file.</returns>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when the file specified was not found</exception>
        Stream OpenWrite(string fileName, bool createNew = false);
    }
}
