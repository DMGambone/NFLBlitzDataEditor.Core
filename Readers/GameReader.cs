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
        protected virtual void LoadGameImage(Stream stream)
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
                throw new ArgumentOutOfRangeException(nameof(address), $"{address} is not a valid memory address");

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
        /// Reads a single image table entry from a stream
        /// </summary>
        /// <param name="reader">An instance of <see cref="BinaryReader" /> to read the data from</param>
        /// <returns>An instance of <see cref="ImageInfo" /></returns>
        protected virtual ImageInfo ReadImageInfo(BinaryReader reader)
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
        public virtual string ReadString(uint address)
        {
            BinaryReader reader = new BinaryReader(OpenMemoryRead(address));
            return reader.ReadAsString();
        }

        /// <inheritdocs />
        public virtual string ReadString(uint address, int size)
        {
            BinaryReader reader = new BinaryReader(OpenMemoryRead(address, size));
            return reader.ReadAsString(size);
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

        /// <summary>
        /// Reads a play route located at the memory address specified
        /// </summary>
        /// <param name="address">The memory address where the play route is located</param>
        /// <returns>An instance of <see cref="PlayRoute" /> containing the route actions</returns>
        protected virtual PlayRoute ReadPlayRoute(uint address)
        {
            BinaryReader reader = new BinaryReader(OpenMemoryRead(address));

            IList<PlayRouteAction> actions = new List<PlayRouteAction>();
            uint routeAction = 0;
            while ((routeAction = reader.ReadUInt32()) != 0)
            {
                byte[] values = BitConverter.GetBytes(routeAction);

                PlayRouteAction action = new PlayRouteAction()
                {
                    X = values[3],
                    Z = values[2],
                    Unknown = values[1],
                    Action = (RouteAction)values[0]
                };

                actions.Add(action);
            }

            return new PlayRoute()
            {
                Actions = actions.ToArray()
            };
        }

        /// <summary>
        /// Reads a single play formation located a memory address specified
        /// </summary>
        /// <param name="address">The memory address where the play formation is located</param>
        /// <returns>An instance of <see cref="PlayStartingPosition" /></returns>
        protected virtual PlayStartingPosition ReadStartingPosition(BinaryReader reader)
        {
            return new PlayStartingPosition()
            {
                X = reader.ReadSingle(),
                Z = reader.ReadSingle(),
                PreSnapActionsAddress = reader.ReadUInt32(),
                Role = reader.ReadInt32(),
                ControlFlag = reader.ReadInt32(),
            };
        }

        /// <summary>
        /// Reads the complete set of play formations located a memory address specified
        /// </summary>
        /// <param name="address">The memory address where the play formations are located</param>
        /// <returns>An collection of <see cref="PlayStartingPosition" /></returns>
        protected virtual PlayStartingPosition[] ReadPlayStartingPositions(uint address)
        {
            int numPlayersPerPlay = _settings.PlayersPerPlay;
            BinaryReader reader = new BinaryReader(OpenMemoryRead(address, numPlayersPerPlay * _settings.PlayFormationRecordSize));

            PlayStartingPosition[] formation = new PlayStartingPosition[numPlayersPerPlay];
            for (int playerIndex = 0; playerIndex < numPlayersPerPlay; playerIndex++)
            {
                formation[playerIndex] = ReadStartingPosition(reader);
            }

            return formation;
        }

        /// <summary>
        /// Reads a complete play located a memory address specified
        /// </summary>
        /// <param name="address">The memory address where the play route is located</param>
        /// <returns>An instance of <see cref="Play" /> containing the complete play</returns>
        protected virtual Play ReadPlay(BinaryReader reader)
        {
            int numPlayersPerPlay = _settings.PlayersPerPlay;

            Play play = new Play()
            {
                PlayFormationAddress = reader.ReadUInt32(),
                Type = (PlayType)reader.ReadUInt32(),
                RouteAddresses = reader.ReadAsUInt32Array(numPlayersPerPlay)
            };

            play.Formation = ReadPlayStartingPositions(play.PlayFormationAddress);

            //Read in the play routes for each player
            play.Routes = new PlayRoute[numPlayersPerPlay];
            uint playerIndex = 0;
            foreach (uint routeAddress in play.RouteAddresses)
            {
                play.Routes[playerIndex] = ReadPlayRoute(routeAddress);
                playerIndex++;
            }

            return play;
        }

        /// <summary>
        /// Reads the next playbook entry from the reader
        /// </summary>
        /// <param name="address">The memory address where the playbook entry is</param>
        /// <returns>An instance of <see cref="PlaybookEntry" /> if there is an entry there.  Null is returned if there is no entry.</returns>
        protected virtual PlaybookEntry ReadPlaybookEntry(uint address)
        {
            return ReadPlaybookEntry(new BinaryReader(OpenMemoryRead(address, _settings.PlaybookEntrySize)));
        }

        /// <summary>
        /// Reads the next playbook entry from the reader
        /// </summary>
        /// <param name="reader">The reader containing the playbook entries</param>
        /// <returns>An instance of <see cref="PlaybookEntry" /> if there is an entry there.  Null is returned if there is no entry.</returns>
        protected virtual PlaybookEntry ReadPlaybookEntry(BinaryReader reader)
        {
            PlaybookEntry entry = new PlaybookEntry()
            {
                NameAddress = reader.ReadUInt32(),
                PlayDataAddress = reader.ReadUInt32(),
                Unknown1 = reader.ReadUInt32(),
                Unknown2 = reader.ReadUInt32(),
                Unknown3 = reader.ReadUInt32()
            };

            if (entry.NameAddress == 0)
                return null;

            entry.Name = ReadString(entry.NameAddress);
            if (entry.PlayDataAddress != 0)
                entry.PlayData = ReadPlay(entry.PlayDataAddress);

            PlayRoute p = ReadPlayRoute(entry.Unknown2);
            return entry;
        }

        /// <summary>
        /// Loads the playbook entries referenced in the reader
        /// </summary>
        /// <param name="reader">A reader containing the collection of playbook entry addresses</param>
        /// <param name="numPlaybookEntries">The number of playbook entries to read from the reader</param>
        /// <returns>A collection of <see cref="PlaybookEntry" /> of all the playbook entries referenced by the addresses in the reader.</returns>
        protected virtual IEnumerable<PlaybookEntry> ReadPlaybookEntries(BinaryReader reader, uint numPlaybookEntries)
        {
            IList<PlaybookEntry> entries = new List<PlaybookEntry>();
            while (entries.Count < numPlaybookEntries)
                entries.Add(ReadPlaybookEntry(reader.ReadUInt32()));

            return entries;
        }

        /// <inheritdocs />
        public virtual Play ReadPlay(uint address)
        {
            BinaryReader reader = new BinaryReader(OpenMemoryRead(address, _settings.PlayRecordSize));

            return ReadPlay(reader);
        }

        /// <inheritdocs />
        public virtual Playbook GetPlaybook()
        {
            IEnumerable<PlaybookEntry> offense;
            IEnumerable<PlaybookEntry> custom;
            IEnumerable<PlaybookEntry> defense;


            using (BinaryReader reader = new BinaryReader(OpenMemoryRead(_settings.PlaybookAddress, _settings.NumberOfPlays * 4)))
            {
                offense = ReadPlaybookEntries(reader, 36);
                custom = ReadPlaybookEntries(reader, 9);
                defense = ReadPlaybookEntries(reader, 18);
            }

            //Get the team specific plays
            IList<IEnumerable<PlaybookEntry>> teamPlays = new List<IEnumerable<PlaybookEntry>>();
            using (BinaryReader reader = new BinaryReader(OpenMemoryRead(_settings.TeamPlaysAddress, (int)(_settings.PlaybookEntrySize * _settings.NumberOfTeamSpecificPlays * _settings.TeamCount))))
            {
                while (!reader.EndOfStream())
                {
                    PlaybookEntry[] plays = new PlaybookEntry[_settings.NumberOfTeamSpecificPlays];
                    for(int i = 0; i < _settings.NumberOfTeamSpecificPlays; i++)
                        plays[i] = ReadPlaybookEntry(reader);
                    teamPlays.Add(plays);
                }
            }

            return new Playbook()
            {
                Offense = offense,
                Defense = defense,
                Custom = custom,
                TeamPlays = teamPlays
            };
        }

        /// <inheritdocs />
        public ImageInfo GetImageInfo(uint address)
        {
            BinaryReader reader = new BinaryReader(OpenMemoryRead(address, 36));
            return ReadImageInfo(reader);
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
                    continue;
                imageInfos.Add(GetImageInfo(imageAddress));
            }

            return imageInfos.ToArray();
        }
    }
}