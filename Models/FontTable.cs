using System.Collections.Generic;

namespace NFLBlitzDataEditor.Core.Models
{
    /// <summary>
    /// Information about a font defined in the game
    /// </summary>
    public class FontTable
    {
        /// <summary>
        /// The total number of characters in the font
        /// </summary>
        public int CharacterCount { get; set; }

        /// <summary>
        /// The ASCII code of the first character in the font
        /// </summary>
        public byte StartCharacter { get; set; }

        /// <summary>
        /// The ASCII code of the last character in the font
        /// </summary>
        public byte EndCharacter { get; set; }

        /// <summary>
        /// The height of a single font row
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The width of a single space in the font
        /// </summary>
        public int SpaceWidth { get; set; }

        /// <summary>
        /// The spacing to use between characters
        /// </summary>
        public int CharacterSpacing { get; set; }

		/// <summary>
		/// The memory address where the name of the image used for the font is stored
		/// </summary>
		public uint FontImageNameAddress { get; set; }

		/// <summary>
		/// The memory address to the array of addresses where the <see cref="ImageInfo" /> are located
		/// </summary>
		public uint CharacterPointersAddress { get; set; }

		/// <summary>
		/// The name of the image used for the font
		/// </summary>
		/// <value></value>
		public string FontImageName { get; set; }

        /// <summary>
        /// A collection of <see cref="ImageInfo" /> identifying each font
        /// </summary>
        public IEnumerable<ImageInfo> Characters { get; set; }

	}
}