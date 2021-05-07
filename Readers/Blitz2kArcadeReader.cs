using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using NFLBlitzDataEditor.Core.Extensions;
using NFLBlitzDataEditor.Core.Enums;
using NFLBlitzDataEditor.Core.Models;

namespace NFLBlitzDataEditor.Core.Readers
{
	public class Blitz2kArcadeReader
		: GameReader
	{
		public static GameFileSettings Settings = new GameFileSettings()
		{
			Version = GameVersion.NFLBlitz2000Arcade,
			PlayerRecordSize = 92,
			PlayersPerTeam = 16,
			TeamListAddress = 0x80185548,
			TeamRecordSize = 116,
			TeamCount = 31,
			FontTables = new uint[] {
				0x80194D04,     // SYSFONT.WMS, 8 height
				0x80194D28,     // SYSFONT.WMS, 9 height
				0x80194D4C,     // SYSFONT.WMS, 11 height
				0x80194D70,     // SYSFONT.WMS, 11 height
				0x80194D94,     // SYSFONT.WMS, 8 height
				0x80194DB8,     // SYSFONT.WMS, 15 height
				0x80194DDC,     // SYSFONT.WMS, 20 height
				0x80194E00,     // SYSFONT.WMS, 33 height
				0x80194E24,     // SYSFONT.WMS, 33 height
				0x80194E48,     // SYSFONT.WMS, 33 height
				0x80194E6C,     // SYSFONT.WMS, 75 height
				0x80194E90,     // FONT23.WMS, 18 height
				0x80194EB4,     // SYSFONT.WMS, 33 height
			}
		};

		public Blitz2kArcadeReader(Stream stream)
			: base(stream, Blitz2kArcadeReader.Settings)
		{
		}

		/// <summary>
		/// Reads the next player record from a stream
		/// </summary>
		/// <param name="reader">An instance of <see cref="BinaryReader"/> to read the record from</param>
		/// <returns>An instance of <see cref="Player" />.</returns>
		protected override Player ReadNextPlayer(BinaryReader reader)
		{
			Player player = new Player();

			player.Stats = reader.ReadAsInt32Array(5);

			//The next int is unused.  Skip it
			reader.ReadInt32();

			//Not sure what Scale is used for, but is consistent for all players.  Might be the texture scale.
			player.Scale = reader.ReadSingle();

			player.Size = (PlayerSize)reader.ReadInt32();
			player.SkinColor = (PlayerSkinColor)reader.ReadInt32();

			//Player number is stored as a integer, which needs to be converted to a hex value to be the correct number.  For example:
			// player #16 is stored as integer value 22, which converted to hex, is 16.
			// Yes, that is odd...
			int numberAsInt = reader.ReadInt32();
			player.Number = int.Parse(numberAsInt.ToString("x2"));

			player.Position = (PlayerPosition)reader.ReadInt32();

			player.MugShotAddress = reader.ReadUInt32();
			player.SelectedNameAddress = reader.ReadUInt32();
			player.NameAddress = reader.ReadUInt32();
			player.Ancr = reader.ReadUInt32();

			player.LastName = reader.ReadAsString(16);
			player.FirstName = reader.ReadAsString(16);

			//resolve the images
			if (player.MugShotAddress != 0)
				player.MugShotImage = GetImageInfo(player.MugShotAddress);
			if (player.SelectedNameAddress != 0)
				player.SelectedNameImage = GetImageInfo(player.SelectedNameAddress);
			if (player.NameAddress != 0)
				player.NameImage = GetImageInfo(player.NameAddress);

			return player;
		}


		/// <summary>
		/// Reads the next team record from a stream
		/// </summary>
		/// <param name="reader">An instance of <see cref="BinaryReader"/> to read the record from</param>
		/// <returns>An instance of <see cref="Team" />.</returns>
		protected override Team ReadNextTeam(BinaryReader reader)
		{
			Team team = new Team();

			team.PassingRating = reader.ReadInt16();
			team.RushingRating = reader.ReadInt16();
			team.LinemenRating = reader.ReadInt16();
			team.DefenseRating = reader.ReadInt16();
			team.SpecialTeamsRating = reader.ReadInt16();
			team.Reserved1 = reader.ReadInt16();

			team.DroneBase = reader.ReadInt32();

			team.Name = reader.ReadAsString(32);
			team.CityName = reader.ReadAsString(32);
			team.CityAbbreviation = reader.ReadAsString(4);
			team.TeamAbbreviation = reader.ReadAsString(4);

			team.PlayersAddress = reader.ReadUInt32();
			team.LogoAddress = reader.ReadUInt32();
			team.Logo30Address = reader.ReadUInt32();
			team.NameAddress = reader.ReadUInt32();
			team.SelectedNameAddress = reader.ReadUInt32();
			team.Reserved2 = reader.ReadUInt32();
			team.LoadingScreenImagesAddress = reader.ReadUInt32();

			//Resolve the image address
			team.LogoImage = GetImageInfo(team.LogoAddress);
			team.Logo30Image = GetImageInfo(team.Logo30Address);
			team.SelectedNameImage = GetImageInfo(team.SelectedNameAddress);
			team.NameImage = GetImageInfo(team.NameAddress);

			using (Stream loadingScreenImagesReader = OpenMemoryRead(team.LoadingScreenImagesAddress))
			{
				//Load the banner images first (max of 2 images)
				team.LoadingScreenBannerImages = GetImageInfo(loadingScreenImagesReader, 2);

				//Get the team name image next (max of 2 images)
				team.LoadingScreenTeamNameImages = GetImageInfo(loadingScreenImagesReader, 2);
			};

			return team;
		}


	}
}