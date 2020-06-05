using System;
using System.Collections.Generic;
using NFLBlitzDataEditor.Core.Enums;

namespace NFLBlitzDataEditor.Core.Models
{
    /// <summary>
    /// The complete playbook in the game
    /// </summary>
    public class Playbook
    {
        /// <summary>
        /// A collection of offensive plays
        /// </summary>
        public IEnumerable<PlaybookEntry> Offense { get; set; } = new PlaybookEntry[0];

        /// <summary>
        /// A collection of defensive plays
        /// </summary>
        public IEnumerable<PlaybookEntry> Defense { get; set; } = new PlaybookEntry[0];

        /// <summary>
        /// A collection of custom built plays
        /// </summary>
        public IEnumerable<PlaybookEntry> Custom { get; set; } = new PlaybookEntry[0];

        /// <summary>
        /// A collection of the team specific plays, 3 per each team
        /// </summary>
        public IList<IEnumerable<PlaybookEntry>> TeamPlays { get; internal set; }
    }
}
