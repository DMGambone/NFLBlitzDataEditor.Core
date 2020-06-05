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
        /// The memory address where the 3 team-specific plays are located 
        /// </summary>
        public uint TeamPlaysAddress { get; set; }

        /// <summary>
        /// The number of team-specific plays per team
        /// </summary>
        public uint NumberOfTeamSpecificPlays { get; set; } = 3;

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
        /// The number of offensive plays
        /// </summary>
        /// <value></value>
        public int NumberOfOffensivePlays { get; set; } = 36;

        /// <summary>
        /// The number of custom plays
        /// </summary>
        /// <value></value>
        public int NumberOfCustomPlays { get; set; } = 36;

        /// <summary>
        /// The number of defensive plays
        /// </summary>
        /// <value></value>
        public int NumberOfDefensivePlays { get; set; } = 36;

        /// <summary>
        /// The size of a single playbook entry
        /// </summary>
        public int PlaybookEntrySize { get; set; } = 20;


        /// <summary>
        /// Returns the number of plays in the playbook
        /// </summary>
        public int NumberOfPlays
        {
            get
            {
                return NumberOfOffensivePlays + NumberOfCustomPlays + NumberOfDefensivePlays;
            }
        }

        /// <summary>
        /// The version of NFL Blitz these settings are for
        /// </summary>
        /// <value></value>
        public GameVersion Version { get; set; }
    }
}