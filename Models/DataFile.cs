using System;
using System.Collections.Generic;

namespace NFLBlitzDataEditor.Core.Models
{
    /// <summary>
    /// Information about the contents of an NFL Blitz data file.
    /// </summary>
    public class DataFile
    {
        /// <summary>
        /// The version of NFL Blitz the data file is for
        /// </summary>
        public NFLBlitzVersion Version { get; internal set; }

        /// <summary>
        /// The list of teams in the data file
        /// </summary>
        public IEnumerable<Team> Teams { get; set; } = new Team[0];
    }
}