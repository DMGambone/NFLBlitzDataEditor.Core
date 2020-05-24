using System;
using System.Linq;
using System.IO;
using NFLBlitzDataEditor.Core.Models;
using NFLBlitzDataEditor.Core.Enums;
using NFLBlitzDataEditor.Core.Extensions;

namespace NFLBlitzDataEditor.Core.Readers
{
    /// <summary>
    /// Used to read information about an image from a stream of data.  Note that this is not the same as the actual image header data.
    /// </summary>
    public class ImageInfoReader
        : IDataFileRecordReader<ImageInfo>
    {
        /// <summary>
        /// Reads a single image table entry from a stream
        /// </summary>
        /// <param name="reader">An instance of <see cref="BinaryReader" /> to read the data from</param>
        /// <returns>An instance of <see cref="ImageInfo" /></returns>
        public ImageInfo Read(BinaryReader reader)
        {
            ImageInfo entry = new ImageInfo();

            entry.ImageAddress = reader.ReadUInt32();

            entry.Width = reader.ReadSingle();
            entry.Height = reader.ReadSingle();

            entry.AnchorX = reader.ReadSingle();
            entry.AnchorY = reader.ReadSingle();

            entry.X1 = reader.ReadSingle();
            entry.X2 = reader.ReadSingle();

            entry.Y1 = reader.ReadSingle();
            entry.Y2 = reader.ReadSingle();

            return entry;
        }
    }
}