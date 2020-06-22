using System;
using System.IO;

namespace MidwayGamesFS
{
    /// <summary>
    /// Provides a basic abstraction to read and write to a collection of bytes
    /// </summary>
    public interface IDataBuffer
    {
        /// <summary>
        /// Returns a collection of bytes 
        /// </summary>
        /// <param name="position">The starting position of the data</param>
        /// <param name="size">The size of the data to read</param>
        /// <returns>A collection of byte from the buffer.</returns>
        byte[] Get(int position, int size);

        /// <summary>
        /// Returns a collection of bytes 
        /// </summary>
        /// <param name="position">The starting position of the data</param>
        /// <param name="size">The size of the data to read</param>
        /// <returns>An instance of <see cref="BinaryReader" /> that can be used to read the data.</returns>
        BinaryReader GetReader(int position, int size);

        /// <summary>
        /// Writes a collection of bytes 
        /// </summary>
        /// <param name="position">The starting position of the data</param>
        /// <param name="data">The data to write</param>
        void Set(int position, byte[] data);

        /// <summary>
        /// Gets a writer that can be used to write to the data
        /// </summary>
        /// <param name="position">The starting position of the data</param>
        /// <returns>An instance of <see cref="BinaryWriter" /> that can be used to write data.</returns>
        BinaryWriter GetWriter(int position);
    }
}