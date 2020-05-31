using System;
using System.IO;
using NFLBlitzDataEditor.Core.Models;
using NFLBlitzDataEditor.Core.Extensions;

namespace NFLBlitzDataEditor.Core.Readers
{
    /// <summary>
    /// Reader used to load a complete image table
    /// </summary>
    public class ImageTableReader
    {
        /// <summary>
        /// The reader used to read a single ImageTableRecord
        /// </summary>
        private IDataFileRecordReader<ImageInfo> TableEntryReader;

        /// <summary>
        /// Initializes the ImageTableReader with the default <see cref="ImageTableRecordReader" />
        /// </summary>
        public ImageTableReader()
            : this(new ImageInfoReader()) { } 

        /// <summary>
        /// Initializes the ImageTableReader with a specified <see cref="IDataFileRecordReader{ImageTableEntry}" />
        /// </summary>
        public ImageTableReader(IDataFileRecordReader<ImageInfo> tableEntryReader)
        {
            TableEntryReader = tableEntryReader;
        }

        /// <summary>
        /// Reads an image table from the reader
        /// </summary>
        /// <param name="reader">An instance of a <see cref="BinaryReader" /> to read the image table from.</param>
        /// <param name="numberOfEntries">The number of entries to read</param>
        /// <returns>An instance of <see cref="ImageTable" /> representing the image table that was read</returns>
        public ImageTable Read(BinaryReader reader, uint numberOfEntries)
        {
            //The first part of the image table record is a string
            string name = reader.ReadAsString(2);

            //Read the ImageInfo entries
            ImageInfo[] entries = new ImageInfo[numberOfEntries];
            for(int i = 0; i < numberOfEntries; i++)
                entries[i] = TableEntryReader.Read(reader);

            return new ImageTable()
            {
                Name = name,
                Entries  = entries
            };
        }
    }
}
