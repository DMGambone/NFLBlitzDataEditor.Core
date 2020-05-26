namespace NFLBlitzDataEditor.Core.Enums
{
    /// <summary>
    /// The image formats used by NFL Blitz
    /// </summary>
    public enum ImageFormat
    {
        /// <summary>
        /// Image is stored as a single byte with Red and Green using 3 bits and Blue using 2 bits.
        /// </summary>
        RGB332 = 0x00,

        /// <summary>
        /// Image is stored in 2 bytes. First byte is the alpha channel followed by a similar format to RGB332.
        /// </summary>
        ARGB8332 = 0x08,

        /// <summary>
        /// Image is stored in 2 bytes with Red and Blue using 5 bits and Green using 6 bits
        /// </summary>
        RGB565 = 0x0a,

        /// <summary>
        /// Image is stored in 2 bytes with the first bit being an alpha indicator and Red, Gree, and Blue all using 5 bits
        /// </summary>
        ARGB1555 = 0x0b,

        /// <summary>
        /// Image is stored in 2 bytes Alpha, Red, Green, Blue all using 4 bits
        /// </summary>
        ARGB4444 = 0x0c,

        /// <summary>
        /// Image stores the alpha levels information as a 16-bit value
        /// </summary>
        AlphaIntensity88 = 0x0d
    }
}