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
        /// The game image loaded into memory
        /// </summary>
        private byte[] _gameImage = null;

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
        protected void LoadGameImage(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            _gameImage = reader.ReadBytes((int)(stream.Length));

            //Read in the memory address offset
            reader = new BinaryReader(new MemoryStream(_gameImage));
            MemoryAddressOffset = reader.ReadUInt32();
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
        private IEnumerable<Player> ReadAllPlayers(BinaryReader reader)
        {
            IList<Player> players = new List<Player>();
            while (players.Count < _settings.PlayersPerTeam)
                players.Add(ReadNextPlayer(reader));

            return players.ToArray();
        }

        /// <inheritdocs />
        public Stream OpenMemoryRead(uint address, int size = -1)
        {
            uint fileOffset = ResolveMemoryAddressToOffset(address);
            if (size == -1)
                size = (int)(_gameImage.Length - fileOffset);

            return new MemoryStream(_gameImage, (int)fileOffset, size);
        }

        /// <inheritdocs />
        public IEnumerable<Team> ReadAllTeams()
        {
            IList<Team> teams = new List<Team>();
            int playerTableSize = _settings.PlayerRecordSize * _settings.PlayersPerTeam;

            //Reset the stream to the start of the teams list
            BinaryReader reader = new BinaryReader(OpenMemoryRead(_settings.TeamListOffset));
            while (teams.Count < _settings.TeamCount)
            {
                //Read the team record
                Team team = ReadNextTeam(reader);

                //Resolve all the memory address pointers
                BinaryReader playersReader = new BinaryReader(OpenMemoryRead(team.PlayersAddress, playerTableSize));
                team.Players = ReadAllPlayers(playersReader);

                team.TeamLogo = GetImageInfo(team.TeamLogoAddress);
                team.TeamLogo30 = GetImageInfo(team.TeamLogo30Address);
                team.TeamSelectionImage = GetImageInfo(team.TeamNameAddress);

                teams.Add(team);
            }

            return teams;
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
        /// Reads a single image table entry from a stream
        /// </summary>
        /// <param name="reader">An instance of <see cref="BinaryReader" /> to read the data from</param>
        /// <returns>An instance of <see cref="ImageInfo" /></returns>
        private ImageInfo ReadImageInfo(BinaryReader reader)
        {
            ImageInfo entry = new ImageInfo();

            uint nameAddress = reader.ReadUInt32();
            string name = ReadString(nameAddress, 12);

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
        public ImageInfo GetImageInfo(uint address)
        {
            BinaryReader reader = new BinaryReader(OpenMemoryRead(address, 36));
            return ReadImageInfo(reader);
        }
    }
}