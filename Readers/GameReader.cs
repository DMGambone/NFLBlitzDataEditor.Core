using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using NFLBlitzDataEditor.Core.Extensions;
using NFLBlitzDataEditor.Core.Models;
using NFLBlitzDataEditor.Core.Enums;

namespace NFLBlitzDataEditor.Core.Readers
{
	public abstract class GameReader
		: IGameReader
	{
		/// <summary>
		/// The game file loaded into memory
		/// </summary>
		private byte[] _gameFile = null;

		/// <summary>
		/// The starting offset of all memory pointers
		/// </summary>
		protected uint MemoryAddressOffset = 0x800C4000;

		/// <summary>
		/// The settings for this particular game file
		/// </summary>
		private readonly GameFileSettings _settings = null;

		/// <summary>
		/// The version of NFL Blitz of this game file
		/// </summary>
		/// <value></value>
		public virtual GameVersion Version
		{
			get
			{
				return _settings.Version;
			}
		}

		public GameReader(Stream stream, GameFileSettings settings)
		{
			_settings = settings;

			LoadGameImage(stream);
		}

		/// <summary>
		/// Load the game into memory and pull out the important information
		/// </summary>
		/// <param name="stream">A stream containing the game file</param>
		protected virtual void LoadGameImage(Stream stream)
		{
			// Pull the complete file into memory
			using(BinaryReader reader = new BinaryReader(stream, System.Text.Encoding.Default, true))
			{
				_gameFile = reader.ReadBytes((int)(stream.Length));
			}

			//Read in the memory address offset
			using(BinaryReader reader = new BinaryReader(new MemoryStream(_gameFile))) 
			{
				MemoryAddressOffset = reader.ReadUInt32();
			}
		}

		/// <inheritdocs />
		protected abstract Team ReadNextTeam(BinaryReader reader);

		/// <inheritdocs />
		protected abstract Player ReadNextPlayer(BinaryReader reader);

		/// <summary>
		/// Resolves a memory address to an offset within the game image
		/// </summary>
		/// <param name="address">The memory address to resolve</param>
		/// <returns>The offset of the game image</returns>
		protected virtual uint ResolveMemoryAddressToOffset(uint address)
		{
			if (address < MemoryAddressOffset)
				throw new ArgumentOutOfRangeException($"{address} is not a valid memory address");

			return (address - MemoryAddressOffset) + 4;
		}

		/// <summary>
		/// Reads all the players from a reader
		/// </summary>
		/// <param name="reader">The reader containing the list of players</param>
		/// <returns>The complete collection of players in the reader</returns>
		protected virtual IEnumerable<Player> ReadAllPlayers(BinaryReader reader)
		{
			IList<Player> players = new List<Player>();
			while (players.Count < _settings.PlayersPerTeam)
				players.Add(ReadNextPlayer(reader));

			return players.ToArray();
		}

		/// <summary>
		/// Returns the string located at a specific memory address
		/// </summary>
		/// <param name="address">The memory address to read the string from</param>
		/// <returns>The string value located at the address</returns>
		protected virtual string ReadString(uint address)
		{
			BinaryReader reader = new BinaryReader(OpenMemoryRead(address));
			return reader.ReadAsString();
		}

		/// <summary>
		/// Returns the string located at a specific memory address
		/// </summary>
		/// <param name="address">The memory address to read the string from</param>
		/// <param name="size">The size of the string</param>
		/// <returns>The string value located at the address</returns>
		protected virtual string ReadString(uint address, int size)
		{
			BinaryReader reader = new BinaryReader(OpenMemoryRead(address, size));
			return reader.ReadAsString(size);
		}

		/// <summary>
		/// Reads the font table information
		/// </summary>
		/// <param name="reader">An instance of <see cref="BinaryReader" /> to read the data from</param>
		/// <returns>An instance of <see cref="FontTable" /></returns>
		protected virtual FontTable ReadFont(BinaryReader reader)
		{
			FontTable fontTable = new FontTable();
			fontTable.CharacterCount = reader.ReadInt32();
			fontTable.StartCharacter = (byte)reader.ReadInt32();
			fontTable.EndCharacter = (byte)reader.ReadInt32();
			fontTable.Height = reader.ReadInt32();
			fontTable.SpaceWidth = reader.ReadInt32();
			fontTable.CharacterSpacing = reader.ReadInt32();
			fontTable.FontImageNameAddress = reader.ReadUInt32();
			fontTable.FontImageName = this.ReadString(fontTable.FontImageNameAddress);
			fontTable.CharacterPointersAddress = reader.ReadUInt32();

			// Get the array of ImageInfo instances for each character
			fontTable.Characters = GetImageInfo(OpenMemoryRead(fontTable.CharacterPointersAddress), (uint)fontTable.CharacterCount);

			return fontTable;
		}

		/// <summary>
		/// Reads a single image table entry from a stream
		/// </summary>
		/// <param name="reader">An instance of <see cref="BinaryReader" /> to read the data from</param>
		/// <returns>An instance of <see cref="ImageInfo" /></returns>
		protected virtual ImageInfo ReadImageInfo(BinaryReader reader)
		{
			ImageInfo entry = new ImageInfo();

			uint nameAddress = reader.ReadUInt32();
			string name = nameAddress != 0 ? ReadString(nameAddress, 12) : null;

			return new ImageInfo()
			{
				ImageNameAddress = nameAddress,
				ImageName = name,

				Width = reader.ReadSingle(),
				Height = reader.ReadSingle(),

				AnchorX = reader.ReadSingle(),
				AnchorY = reader.ReadSingle(),

				X1 = reader.ReadSingle(),
				X2 = reader.ReadSingle(),

				Y1 = reader.ReadSingle(),
				Y2 = reader.ReadSingle()
			};
		}

		/// <inheritdocs />
		public Stream OpenMemoryRead(uint address, int size = -1)
		{
			uint fileOffset = ResolveMemoryAddressToOffset(address);
			if (size == -1)
				size = (int)(_gameFile.Length - fileOffset);

			return new MemoryStream(_gameFile, (int)fileOffset, size);
		}

		/// <inheritdocs />
		public IEnumerable<Team> ReadAllTeams()
		{
			IList<Team> teams = new List<Team>();
			int playerTableSize = _settings.PlayerRecordSize * _settings.PlayersPerTeam;

			//Reset the stream to the start of the teams list
			BinaryReader reader = new BinaryReader(OpenMemoryRead(_settings.TeamListAddress));
			while (teams.Count < _settings.TeamCount)
			{
				//Read the team record
				Team team = ReadNextTeam(reader);

				//Resolve all the memory address pointers
				BinaryReader playersReader = new BinaryReader(OpenMemoryRead(team.PlayersAddress, playerTableSize));
				team.Players = ReadAllPlayers(playersReader);

				teams.Add(team);
			}

			return teams;
		}

		/// <inheritdocs />
		public ImageInfo GetImageInfo(uint address)
		{
			BinaryReader reader = new BinaryReader(OpenMemoryRead(address, 36));
			return ReadImageInfo(reader);
		}

		/// <inheritdocs />
		public FontTable GetFont(uint address)
		{
			BinaryReader reader = new BinaryReader(OpenMemoryRead(address));
			return ReadFont(reader);
		}

		/// <summary>
		/// Returns a list of <see cref="ImageInfo" /> from a stream of image addresses
		/// </summary>
		/// <param name="reader">The reader that will contain the image address</param>
		/// <param name="count">The number of images to pull from the stream</param>
		/// <returns>A collection of <see cref="ImageInfo" /> based on the stream of image address. Address value of 0 are skipped.</returns>
		protected IEnumerable<ImageInfo> GetImageInfo(Stream imageAddressStream, uint count)
		{
			IList<ImageInfo> imageInfos = new List<ImageInfo>();
			BinaryReader imageAddressReader = new BinaryReader(imageAddressStream);
			while ((count--) > 0)
			{
				uint imageAddress = imageAddressReader.ReadUInt32();
				if (imageAddress == 0)
					imageInfos.Add(null);
				else
					imageInfos.Add(GetImageInfo(imageAddress));
			}

			return imageInfos.ToArray();
		}

		public IEnumerable<FontTable> GetAllFonts()
		{
			IList<FontTable> fontTables = new List<FontTable>();
			foreach(uint fontTableAddress in _settings.FontTables)
				fontTables.Add(GetFont(fontTableAddress));

			return fontTables;
		}
	}
}