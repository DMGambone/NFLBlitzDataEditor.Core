using System.Linq;
using System.Collections.Generic;
using System.IO;
using NFLBlitz2kDataEditor.Models;

namespace NFLBlitz2kDataEditor.Readers
{
    public abstract class DataFileReaderBase
    {
        /// <summary>
        /// An instance of a BinaryReader that allows for pulling data from the Blitz data file
        /// </summary>
        private System.IO.BinaryReader _reader = null;

        /// <summary>
        /// The settings for this particular data file
        /// </summary>
        protected DataFileSettings Settings { get; private set; }

        /// <summary>
        /// The data file version the reader supports
        /// </summary>
        protected abstract NFLBlitzVersion Version { get; }

        public DataFileReaderBase(System.IO.Stream stream, DataFileSettings settings)
        {
            _reader = new BinaryReader(stream, System.Text.Encoding.ASCII);
            Settings = settings;
        }

        /// <summary>
        /// Reads the next set of bytes and converts it into a string
        /// </summary>
        /// <param name="reader">The reader to read the bytes from</param>
        /// <param name="length">The length of the string to read</param>
        /// <returns>A string containing the data that was read.  Any trailing null characters are trimmed.</returns>
        protected string ReadAsString(System.IO.BinaryReader reader, int length)
        {
            byte[] bytes = reader.ReadBytes(length);
            string value = System.Text.Encoding.ASCII.GetString(bytes);
            return value.TrimEnd((char)0);
        }

        /// <summary>
        /// Reads an array of int values
        /// </summary>
        /// <param name="reader">The reader to read the int from</param>
        /// <param name="length">The number of int values to read</param>
        /// <returns>An array of int values.</returns>
        protected int[] ReadAsInt32Array(System.IO.BinaryReader reader, int length)
        {
            int[] values = new int[length];
            for (int i = 0; i < length; i++)
            {
                values[i] = reader.ReadInt32();
            }

            return values;
        }

        /// <summary>
        /// Reads an array of unsigned int values
        /// </summary>
        /// <param name="reader">The reader to read the int from</param>
        /// <param name="length">The number of values to read</param>
        /// <returns>An array of unsigned int values.</returns>
        protected uint[] ReadAsUInt32Array(System.IO.BinaryReader reader, int length)
        {
            uint[] values = new uint[length];
            for (int i = 0; i < length; i++)
            {
                values[i] = reader.ReadUInt32();
            }

            return values;
        }

        /// <summary>
        /// Reads the next set of bytes until it reaches a null character
        /// </summary>
        /// <param name="reader">The reader to read the bytes from</param>
        /// <returns>A string containing the data that was read.</returns>
        protected virtual string ReadAsString(System.IO.BinaryReader reader)
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

            while (reader.PeekChar() != -1)
            {
                char nextCharacter = reader.ReadChar();
                if (nextCharacter == 0x00)
                    break;

                stringBuilder.Append(nextCharacter);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Returns a reader that contains the team data for the next team
        /// </summary>
        /// <param name="recordSize">The size of the team data block</param>
        /// <returns>An instance of a BinaryReader that can be used to read the team record.</returns>
        protected System.IO.BinaryReader ReadNextBlock(BinaryReader reader, int recordSize)
        {
            byte[] block = reader.ReadBytes(recordSize);

            return new BinaryReader(new MemoryStream(block));
        }

        /// <summary>
        /// Reads the next team record from a stream
        /// </summary>
        /// <param name="reader">The reader to read the next team record from</param>
        /// <returns>An instance of <see cref="Team" />.  If there is no team record to be read, null is returned.</returns>
        protected abstract Team ReadNextTeam(BinaryReader reader);

        /// <summary>
        /// Reads the next player record from a stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>An instance of <see cref="Player" />.  If there is no player record to read, null is returned</returns>
        protected abstract Player ReadNextPlayer(BinaryReader reader);

        /// <summary>
        /// Reads the complete set of teams defined in the data file
        /// </summary>
        /// <returns>A collection of teams defined in the data file</returns>
        public IEnumerable<Team> ReadAllTeams()
        {
            IList<Team> teams = new List<Team>();

            //Reset the stream to the start of the teams list
            _reader.BaseStream.Seek(Settings.TeamListOffset, SeekOrigin.Begin);
            while (teams.Count < Settings.TeamCount)
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
            _reader.BaseStream.Seek(Settings.PlayerListOffset, SeekOrigin.Begin);
            while (true)
            {
                if (players.Count == 496)
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
                team.Players = players.Skip(teamIndex * Settings.PlayersPerTeam).Take(Settings.PlayersPerTeam).ToArray();
                teamIndex++;
            }

            return new DataFile()
            {
                Version = Version,
                Teams = teams
            };
        }
    }
}