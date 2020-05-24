using System;
using System.Collections.Generic;

namespace NFLBlitzDataEditor.Core.Models
{
    /// <summary>
    /// An image table is a collection of <see cref="ImageInfo" /> entries that point to section within a larger spritesheet image.
    /// </summary>
    public class ImageTable
    {
        /// <summary>
        /// The name of the image table
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The individual records within the image table
        /// </summary>
        public IEnumerable<ImageInfo> Entries { get; set; } = new ImageInfo[0];
    }
}