namespace NFLBlitzDataEditor.Core.Enums
{
    /// <summary>
    /// Sets the type of map map filter to use when applying the texture
    /// </summary>
    public enum MipMapFilterMode
    {
        /// <summary>
        /// No mip-mapping is used
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Use the nearest mip-map
        /// </summary>
        Nearest = 1,

        /// <summary>
        /// use the nearest mip-map with dithering applied
        /// </summary>
        NearestDither = 2,
    }
}