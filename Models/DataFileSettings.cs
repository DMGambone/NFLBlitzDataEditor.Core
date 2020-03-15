namespace NFLBlitzDataEditor.Core.Models
{
    /// <summary>
    /// Information about an NFL data file that is to be read
    /// </summary>
    public class DataFileSettings
    {
        /// <summary>
        /// The starting position of the teams list in the data file
        /// </summary>
        public uint TeamListOffset { get; set; }

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
        /// The starting position of the players list in the data file
        /// </summary>
        /// <value></value>
        public uint PlayerListOffset { get; set; }

        /// <summary>
        /// The size of a single player record
        /// </summary>
        public int PlayerRecordSize { get; set; }

        /// <summary>
        /// The number of players per team
        /// </summary>
        public int PlayersPerTeam { get; set; }
    }
}