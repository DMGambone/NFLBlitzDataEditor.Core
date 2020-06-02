namespace NFLBlitzDataEditor.Core.Models
{
    /// <summary>
    /// Information about an NFL data file that is to be read
    /// </summary>
    public class GameFileSettings
    {
        /// <summary>
        /// The memory address of the teams list in the data file
        /// </summary>
        public uint TeamListAddress { get; set; }

        /// <summary>
        /// The size of a single team reacord
        /// </summary>
        public int TeamRecordSize { get; set; }

        /// <summary>
        /// The number of teams in the data file
        /// </summary>
        /// <value></value>
        public uint TeamCount { get; set; }

        /// <summary>
        /// The size of a single player record
        /// </summary>
        public int PlayerRecordSize { get; set; }

        /// <summary>
        /// The number of players per team
        /// </summary>
        public int PlayersPerTeam { get; set; }

        /// <summary>
        /// The version of NFL Blitz these settings are for
        /// </summary>
        /// <value></value>
        public GameVersion Version { get; internal set; }
    }
}