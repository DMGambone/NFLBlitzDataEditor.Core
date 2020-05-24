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

        /// <summary>
        /// Pointer to a player's image (aka: mugshot)
        /// </summary>
        public uint MugShotAddress { get; set; }

        /// <summary>
        /// Pointer to the player's name
        /// </summary>
        public uint NameAddress { get; set; }

        /// <summary>
        /// Pointer to a player's name when selected
        /// </summary>
        public uint SelectedNameAddress { get; set; }

        /// <summary>
        /// Identified as "ancr" in NFL Blitz code, but it's purpose is unknown
        /// </summary>
        public uint Ancr { get; set; }
    }
}