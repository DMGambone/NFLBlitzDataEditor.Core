using System;
using NFLBlitzDataEditor.Core.Enums;

namespace NFLBlitzDataEditor.Core.Models
{
    /// <summary>
    /// Represents a single image file in NFL Blitz, which is often a composite of multiple images
    /// </summary>
    public class Image
    {
        /// <summary>
        /// The image table containing the breakdown of the NFL Blitz image file
        /// </summary>
        public ImageTable Info { get; set; }

        /// <summary>
        /// The location within the data file where the image table was located
        /// </summary>
        public long ImageTableLocation { get; set; }

        /// <summary>
        /// The number of entries in the image table
        /// </summary>
        public uint ImageTableSize { get; set; }

        /// <summary>
        /// Contains the actual image data
        /// </summary>
        public ImageData Data { get; set; }

        /// <summary>
        /// The location within data file where the image data was located
        /// </summary>
        public long ImageDataLocation { get; set; }
    }
}