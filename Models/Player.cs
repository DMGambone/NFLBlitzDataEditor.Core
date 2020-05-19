using System;
using NFLBlitzDataEditor.Core.Enums;

namespace NFLBlitzDataEditor.Core.Models
{
    /// <summary>
    /// Contains information about a single player
    /// </summary>
    public class Player
    {
        /// <summary>
        /// The player's first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The player's last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Player stats?: Spd, Pwr, QB, WR, Off, Def
        /// </summary>
        public int[] Stats { get; set; }

        /// <summary>
        /// Scale of texture?
        /// </summary>
        public float Scale { get; internal set; }
        
        /// <summary>
        /// The player's skin color
        /// </summary>
        public PlayerSkinColor SkinColor { get; set; }

        /// <summary>
        /// The player's game number
        /// </summary>
        /// <value></value>
        public int Number { get; set; }

        /// <summary>
        /// Indicates the size of the player
        /// </summary>
        public PlayerSize Size { get; set; }

        /// <summary>
        /// The position of the player
        /// </summary>
        public PlayerPosition Position { get; set; }

        public uint UnknownAddr1 { get; set; }

        public int UnknownValue1 { get; set; }
        public int UnknownValue2 { get; set; }

        public uint UnknownAddr2 { get; set; }

    }
}