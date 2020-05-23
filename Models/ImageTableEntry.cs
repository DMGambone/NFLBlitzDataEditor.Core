using System;
using System.Collections.Generic;

namespace NFLBlitzDataEditor.Core.Models
{
    /// <summary>
    /// A single entry in an image table.
    /// </summary>
    public class ImageTableEntry
    {
        /// <summary>
        /// The width of the image
        /// </summary>
        public float Width { get; set; }
        
        /// <summary>
        /// The height of the image
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float ax { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float ay { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float s_start { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float s_end { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float t_start { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float t_end { get; set; }
    }
}