using System;
using NFLBlitzDataEditor.Core.Enums;

namespace NFLBlitzDataEditor.Core.Models
{
    /// <summary>
    /// Contains information about a player's starting position for a play
    /// </summary>
    public class PlayStartingPosition
    {
        /// <summary>
        /// A players starting position offset from the line of scrimmage (ie: distance away from the line of scrimmage)
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// A players position along the line of scrimmage
        /// </summary>
        public float Z { get; set; }

        /// <summary>
        /// A sequence of pre-snap actions the player is to take
        /// </summary>
        public uint PreSnapActionsAddress { get; set; }

        /// <summary>
        /// The role of the player
        /// </summary>
        public int Role { get; set; }

        /// <summary>
        /// Pre-snap control flags
        /// </summary>
        public int ControlFlag {get; set; }
    }
}