using System.Collections.Generic;

namespace NFLBlitzDataEditor.Core.Models
{
    /// <summary>
    /// Contains information about a single team in NFL Blitz, including the players, images, and sounds associated with the team.
    /// </summary>
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
        /// Unused value in the team record
        /// </summary>
        public int Reserved1 { get; set; }

        /// <summary>
        /// Base setup of the team AI
        /// </summary>
        public int DroneBase { get; set; }

        /// <summary>
        /// Points to the memory location of the players for this team
        /// </summary>
        public uint PlayersAddress { get; set; }

        /// <summary>
        /// Points to the memory location of the team logo
        /// </summary>
        public uint LogoAddress { get; set; }

        /// <summary>
        /// Points to the memory location of the reduced size team logo
        /// </summary>
        public uint Logo30Address { get; set; }

        /// <summary>
        /// Points to the memory location of the image used to select the team
        /// </summary>
        public uint NameAddress { get; set; }

        /// <summary>
        /// Points to the memory location of the image used to select the team and the team is selected
        /// </summary>
        public uint SelectedNameAddress { get; set; }

        /// <summary>
        /// Always 0 (Reserved)?
        /// </summary>
        public uint Reserved2 { get; set; }

        /// <summary>
        /// Pointer to the a list of <see cref="ImageInfo" /> that are the images used on the loading screen for.
        /// Appears to be 3 in total
        /// </summary>
        public uint LoadingScreenImagesAddress { get; set; }

        #region Resolved properties
        /// <summary>
        /// The players that are on a team
        /// </summary>
        public IEnumerable<Player> Players { get; set; } = new Player[0];

        /// <summary>
        /// The team logo image
        /// </summary>
        public ImageInfo LogoImage { get; set; }

        /// <summary>
        /// The team logo image??
        /// </summary>
        public ImageInfo Logo30Image { get; set; }

        /// <summary>
        /// The team name image
        /// </summary>
        public ImageInfo SelectedNameImage { get; set; }

        /// <summary>
        /// The team name image when selected
        /// </summary>
        public ImageInfo NameImage { get; set; }

        /// <summary>
        /// The images used for the banner at the very top of the loading screen
        /// </summary>
        public IEnumerable<ImageInfo> LoadingScreenBannerImages { get; set; } = new ImageInfo[0];

        /// <summary>
        /// The images used for the team name below the banner on the loading screen
        /// </summary>
        public IEnumerable<ImageInfo> LoadingScreenTeamNameImages { get; set; } = new ImageInfo[0];
        #endregion
    }
}