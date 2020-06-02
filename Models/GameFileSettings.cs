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
        public int PlayersPerTeam { get; set; } = 16;

        /// <summary>
        /// The memory address to the playbook
        /// </summary>
        public uint PlaybookAddress { get; set; }

        /// <summary>
        /// The number of players involved in a play
        /// </summary>
        public int PlayersPerPlay { get; set; } = 7;

        /// <summary>
        /// The size of a play formation record
        /// </summary>
        public int PlayFormationRecordSize { get; set; } = 20;

        /// <summary>
        /// The size of a play record
        /// </summary>
        public int PlayRecordSize { get; set; } = 36;

        /// <summary>
        /// The number of plays in the playbook
        /// </summary>
        public int NumberOfPlays { get; set; } = 60;

        /// <summary>
        /// The version of NFL Blitz these settings are for
        /// </summary>
        /// <value></value>
        public GameVersion Version { get; set; }
    }
}