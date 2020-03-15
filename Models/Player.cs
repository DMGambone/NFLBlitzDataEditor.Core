using System;
using NFLBlitzDataEditor.Core.Enums;

namespace NFLBlitzDataEditor.Core.Models
{
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

        public int[] UnknownRegion1 { get; set; }
    }
}