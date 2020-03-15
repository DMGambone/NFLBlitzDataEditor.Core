using System;
using NFLBlitzDataEditor.Core.Enums;

namespace NFLBlitzDataEditor.Core.Models
{
    /// <summary>
    /// Represents a single image in NFL Blitz
    /// </summary>
    public class Image
    {
        /// <summary>
        /// A file type or header version indicator?
        /// </summary>
        public uint FileType { get; set; }

        public uint UnknownValue1 { get; set; }
        public uint UnknownValue2 { get; set; }
        public uint UnknownValue3 { get; set; }

        /// <summary>
        /// The width of the image
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the image
        /// </summary>
        public int Height { get; set; }

        public uint MipmappingLevel { get; set; }

        public uint UnknownValue4 { get; set; }

        public uint UnknownValue5 { get; set; }

        /// <summary>
        /// The format the image is in
        /// </summary>
        public ImageFormat Format { get; set; }

        /// <summary>
        /// The image pixels
        /// </summary>
        public uint[] Data { get; set; }
    }
}