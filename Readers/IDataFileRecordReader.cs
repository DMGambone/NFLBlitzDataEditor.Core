using System.IO;
using NFLBlitzDataEditor.Core.Models;

namespace NFLBlitzDataEditor.Core.Readers
{
    /// <summary>
    /// A common inferface for reading in a block of data as a record from an NFL Blitz data file
    /// </summary>
    /// <typeparam name="T">The type of data returned from the reader</typeparam>
    public interface IDataFileRecordReader<T>
    {
        /// <summary>
        /// Reads the record from a stream
        /// </summary>
        /// <param name="reader">An instance of <see cref="BinaryReader"/> to read the record from</param>
        /// <returns>An instance of <see cref="T" /> representing the data that was read.</returns>
        T Read(BinaryReader reader);
    }
}