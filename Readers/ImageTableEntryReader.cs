using System;
using System.Linq;
using System.IO;
using NFLBlitzDataEditor.Core.Models;
using NFLBlitzDataEditor.Core.Enums;
using NFLBlitzDataEditor.Core.Extensions;

namespace NFLBlitzDataEditor.Core.Readers
{
    /// <summary>
    /// Used to read an image table entry from a stream of data
    /// </summary>
    public class ImageTableEntryReader
        : IDataFileRecordReader<ImageTableEntry>
    {
        /// <summary>
        /// Reads a single image table entry from a stream
        /// </summary>
        /// <param name="reader">An instance of <see cref="BinaryReader" /> to read the data from</param>
        /// <returns>An instance of <see cref="ImageTableEntry" /></returns>
        public ImageTableEntry Read(BinaryReader reader)
        {
            ImageTableEntry entry = new ImageTableEntry();

            entry.Width = reader.ReadSingle();
            entry.Height = reader.ReadSingle();

            entry.ax = reader.ReadSingle();
            entry.ay = reader.ReadSingle();

            entry.s_start = reader.ReadSingle();
            entry.s_end = reader.ReadSingle();

            entry.t_start = reader.ReadSingle();
            entry.t_end = reader.ReadSingle();

            return entry;
        }
    }
}