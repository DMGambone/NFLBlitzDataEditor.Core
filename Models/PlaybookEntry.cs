using System;
using System.Collections.Generic;
using NFLBlitzDataEditor.Core.Enums;

namespace NFLBlitzDataEditor.Core.Models
{
    /// <summary>
    /// The signle entry in the playbook
    /// </summary>
    public class PlaybookEntry
    {
        /// <summary>
        /// The address of the entry's name
        /// </summary>
        public uint NameAddress { get; set; }

        /// <summary>
        /// The address of the play data
        /// </summary>
        public uint PlayDataAddress { get; internal set; }

        public uint Unknown1 { get; set; }

        public uint Unknown2 { get; set; }

        public uint Unknown3 { get; set; }

        /// <summary>
        /// The name of the play
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The actual play for this entry
        /// </summary>
        public Play PlayData { get; set; }
    }
}
