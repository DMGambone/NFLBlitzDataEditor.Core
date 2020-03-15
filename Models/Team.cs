using System.Collections.Generic;

namespace NFLBlitzDataEditor.Core.Models
{
    public class Team
    {
        /// <summary>
        /// Name of the team
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The abbreviation of the team name
        /// </summary>
        public string TeamAbbreviation { get; internal set; }

        /// <summary>
        /// The name of the city the team belongs to
        /// </summary>
        public string CityName { get; internal set; }

        /// <summary>
        /// The abbreviation of the city the team belongs to
        /// </summary>
        public string CityAbbreviation { get; internal set; }

        /// <summary>
        /// The players that are on a team
        /// </summary>
        public IEnumerable<Player> Players { get; set; } = new Player[0];

        /// <summary>
        /// The team's offensive passing rating
        /// </summary>
        public int PassingRating { get; set; }

        /// <summary>
        /// The team's offensive rushing rating
        /// </summary>
        public int RushingRating { get; set; }

        /// <summary>
        /// The team's offensive linemen rating
        /// </summary>
        public int LinemenRating { get; set; }

        /// <summary>
        /// The team's defensive rating
        /// </summary>
        public int DefenseRating { get; set; }

        /// <summary>
        /// The team's special teams rating
        /// </summary>
        public int SpecialTeamsRating { get; set; }

        /// <summary>
        /// Unknown purpose (bytes 10-15)
        /// </summary>
        public byte[] UnknownRegion1 { get; set; }

        /// <summary>
        /// Points to a 1472 byte section?
        /// </summary>
        public uint FileOffset1 { get; set; }
        /// <summary>
        /// Points to a 48 byte section?
        /// </summary>
        public uint FileOffset2 { get; set; }
        /// <summary>
        /// Points to a 36 or 52 byte section?
        /// </summary>
        public uint FileOffset3 { get; set; }
        /// <summary>
        /// Points to a 36 or 1344 (only the Ravens) byte section?
        /// </summary>
        public uint FileOffset4 { get; set; }
        /// <summary>
        /// Points to a 36 or 1668 (only the Bengals) byte section?
        /// </summary>
        public uint FileOffset5 { get; set; }
        /// <summary>
        /// Always 0 (Reserved)?
        /// </summary>
        public uint FileOffset6 { get; set; }
        /// <summary>
        /// Points to a 16 or 20 byte section?
        /// </summary>
        public uint FileOffset7 { get; set; }
    }
}