namespace NFLBlitzDataEditor.Core.Models
{
    /// <summary>
    /// Game version information.  This is stored as 32 bytes in the GAMEREV.INF file
    /// </summary>
    public class GameVersion
    {
        /// <summary>
        /// A 32-bit value, but purpose is currently unknown
        /// </summary>
        public uint UnknownValue { get; set; }

        /// <summary>
        /// The name of the NFL Blitz Version
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        #region Pre-defined game versions
        public static readonly GameVersion NFLBlitz2000Arcade = new GameVersion()
        {
            UnknownValue = 1276313856,
            Name = "NFL Blitz 2000 - SEATTLE"
        };
        #endregion
    }
}