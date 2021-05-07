using System.Collections.Generic;

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
        public GameVersion Version { get; set; }

		/// <summary>
		/// The location of the font tables used in the NFL Blitz.  Each entry is the name of the font 
		/// and the memory address to that font
		/// </summary>
		public IEnumerable<uint> FontTables { get; set; }
    }
}