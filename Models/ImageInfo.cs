using System;
using System.Collections.Generic;

namespace NFLBlitzDataEditor.Core.Models
{
    /// <summary>
    /// Contains information about the location of an image and how it's applied when rendered.
    /// </summary>
    public class ImageInfo
    {
        /// <summary>
        /// A pointer to the actual image
        /// </summary>
        /// <value></value>
        public uint ImageAddress { get; set; }

        /// <summary>
        /// The width of the image
        /// </summary>
        public float Width { get; set; }
        
        /// <summary>
        /// The height of the image
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// The horizontal offset to use when placing the image
        /// </summary>
        public float AnchorX { get; set; }

        /// <summary>
        /// The vertical offset to use when placing the image
        /// </summary>
        public float AnchorY { get; set; }

        /// <summary>
        /// The horizontal starting position of the image in a spritesheet source image.  X1 * Width provides the pixel location.
        /// </summary>
        public float X1 { get; set; }

        /// <summary>
        /// The horizontal ending position of the image in a spritesheet source image.  X2 * Width provides the pixel location.
        /// </summary>
        public float X2 { get; set; }

        /// <summary>
        /// The vertical starting position of the image in a spritesheet source image.  Y1 * Height provides the pixel location.
        /// </summary>
        public float Y1 { get; set; }

        /// <summary>
        /// The vertical ending position of the image in a spritesheet source image.  Y2 * Height provides the pixel location.
        /// </summary>
        public float Y2 { get; set; }
    }
}