using System;
using NFLBlitzDataEditor.Core.Enums;

namespace NFLBlitzDataEditor.Core.Models
{
    /// <summary>
    /// Represents a single image in NFL Blitz
    /// </summary>
    public class ImageData
    {
        /// <summary>
        /// A image header version indicator
        /// </summary>
        public uint Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float Bias { get; set; }

        /// <summary>
        /// Mip-map mode
        /// </summary>
        public MipMapFilterMode FilterMode { get; set; }

        /// <summary>
        /// Indicates if to use Trilinear filtering when going between mipmaps
        /// </summary>
        public bool UseTrilinearFiltering { get; set; }

        /// <summary>
        /// The width of the image
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the image
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The smallest level of detail in the image data
        /// </summary>
        public LevelOfDetail SmallestLOD { get; set; }

        /// <summary>
        /// The larges level of detail in the image data
        /// </summary>
        public LevelOfDetail LargestLOD { get; set; }

        /// <summary>
        /// The aspect ratio of the textures
        /// </summary>
        public LODAspectRatio AspectRatio { get; set; }

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