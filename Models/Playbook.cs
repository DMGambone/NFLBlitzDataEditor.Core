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
        public IEnumerable<PlaybookEntry> Offense = new PlaybookEntry[0];

        public IEnumerable<PlaybookEntry> Defense = new PlaybookEntry[0];
    }
}
