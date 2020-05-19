using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using NFLBlitzDataEditor.Core.Models;
using NFLBlitzDataEditor.Core.Enums;

namespace NFLBlitzDataEditor.Core.Readers
{
    public class DataFileReader
        : IDataFileReader
    {
        /// <summary>
        /// An instance of a BinaryReader that allows for pulling data from the Blitz data file
        /// </summary>
        private readonly BinaryReader _reader = null;

        /// <summary>
        /// The settings for this particular data file
        /// </summary>
        private readonly DataFileSettings _settings = null;

        /// <summary>
        /// Reader used to read image records from the data file
        /// </summary>
        private readonly IDataFileRecordReader<Image> _imageRecordReader;

        /// <summary>
        /// Reader used to read player records from the data file
        /// </summary>
        private readonly IDataFileRecordReader<Player> _playerRecordReader;

        /// <summary>
        /// Reader used to read team records from the data file
        /// </summary>
        private readonly IDataFileRecordReader<Team> _teamRecordReader;

        /// <summary>
        /// The version of NFL Blitz this datafile is reading
        /// </summary>
        /// <value></value>
        public virtual NFLBlitzVersion Version
        {
            get
            {
                return _settings.Version;
            }
        }

        public DataFileReader(Stream stream, DataFileSettings settings, IDataFileRecordReader<Image> imageRecordReader, IDataFileRecordReader<Player> playerRecordReader, IDataFileRecordReader<Team> teamRecordReader)
        {
            _reader = new BinaryReader(stream, System.Text.Encoding.ASCII);
            _imageRecordReader = imageRecordReader;
            _playerRecordReader = playerRecordReader;
            _teamRecordReader = teamRecordReader;
            _settings = settings;
        }

        /// <summary>
        /// Reads the next team record from a stream
        /// </summary>
        /// <param name="reader">The reader to read the next team record from</param>
        /// <returns>An instance of <see cref="Team" />.  If there is no team record to be read, null is returned.</returns>
        protected virtual Team ReadNextTeam(BinaryReader reader)
        {
            return _teamRecordReader.Read(reader);
        }

        /// <summary>
        /// Reads the next player record from a stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>An instance of <see cref="Player" />.  If there is no player record to read, null is returned</returns>
        protected virtual Player ReadNextPlayer(BinaryReader reader)
        {
            return _playerRecordReader.Read(reader);
        }

        /// <summary>
        /// Reads the complete set of teams defined in the data file
        /// </summary>
        /// <returns>A collection of teams defined in the data file</returns>
        public IEnumerable<Team> ReadAllTeams()
        {
            IList<Team> teams = new List<Team>();

            //Reset the stream to the start of the teams list
            _reader.BaseStream.Seek(_settings.TeamListOffset, SeekOrigin.Begin);
            while (teams.Count < _settings.TeamCount)
            {
                Team team = ReadNextTeam(_reader);
                teams.Add(team);
            }

            return teams;
        }

        /// <summary>
        /// Reads in the complete list of players in the data file
        /// </summary>
        /// <returns>A collection of players defined in the data file</returns>
        public IEnumerable<Player> ReadAllPlayers()
        {
            IList<Player> players = new List<Player>();

            //Reset the stream to the start of the players list
            _reader.BaseStream.Seek(_settings.PlayerListOffset, SeekOrigin.Begin);
            while (true)
            {
                if (players.Count == _settings.TeamCount * _settings.PlayersPerTeam)
                    break;

                Player player = ReadNextPlayer(_reader);
                players.Add(player);
            }

            return players;
        }

        /// <summary>
        /// Reads the data file and returns data stored in that file
        /// </summary>
        /// <returns>An instance of <see cref="DataFile" /> containing information about the data file</returns>
        public DataFile Read()
        {
            //Get the teams and players, and then merge them together
            IEnumerable<Team> teams = ReadAllTeams();
            IEnumerable<Player> players = ReadAllPlayers();

            int teamIndex = 0;
            foreach (Team team in teams)
            {
                team.Players = players.Skip(teamIndex * _settings.PlayersPerTeam).Take(_settings.PlayersPerTeam).ToArray();
                teamIndex++;
            }

            return new DataFile()
            {
                Version = Version,
                Teams = teams
            };
        }

        /// <summary>
        /// Returns the image data located at a location in the file
        /// </summary>
        /// <param name="position">The starting position to read the data</param>
        /// <returns>An instance of <see cref="Image" />.  If there is no valid image data, null is returned.</returns>
        public Image ReadImage(long position)
        {
            //First grab the first 40 bytes (as UInt32).  This is the header record.
            _reader.BaseStream.Seek(position, SeekOrigin.Begin);

            return _imageRecordReader.Read(_reader);
        }
    }
}