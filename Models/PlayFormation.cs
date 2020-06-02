using System;
using NFLBlitzDataEditor.Core.Enums;

namespace NFLBlitzDataEditor.Core.Models
{
    /// <summary>
    /// Contains information about a player's starting formation for a play
    /// </summary>
    public class PlayFormation
    {
        /// <summary>
        /// A players starting position offset from the line of scrimmage (ie: distance away from the line of scrimmage)
        /// </summary>
        public float LineOfScrimmageX { get; set; }

        /// <summary>
        /// A players position along the line of scrimmage
        /// </summary>
        public float LineOfScrimmageZ { get; set; }

        /// <summary>
        /// A sequence of actions the player is to take
        /// </summary>
        public byte[] Sequence { get; set; }

        /// <summary>
        /// ??
        /// </summary>
        public int Mode { get; set; }

        /// <summary>
        /// Pre-snap control flags
        /// </summary>
        public int ControlFlag {get; set; }
    }
}