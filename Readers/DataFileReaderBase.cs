using System.Linq;
using System.Collections.Generic;
using System.IO;
using NFLBlitz2kDataEditor.Models;
using NFLBlitz2kDataEditor.Enums;

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
        /// <param name="reader">The reader to read the bytes from</param>
        /// <param name="recordSize">The size of the team data block</param>
        /// <returns>An instance of a BinaryReader that can be used to read the team record.</returns>
        protected System.IO.BinaryReader ReadNextBlock(BinaryReader reader, int recordSize)
        {
            byte[] block = reader.ReadBytes(recordSize);

            return new BinaryReader(new MemoryStream(block));
        }

        /// <summary>
        /// Reads an array of UInt16 values
        /// </summary>
        /// <param name="reader">The reader to read the UInt16 from</param>
        /// <param name="length">The number of values to read</param>
        /// <returns>An array of unsigned UInt16 values.</returns>
        protected ushort[] ReadAsUInt16Array(BinaryReader reader, int length)
        {
            ushort[] values = new ushort[length];
            for (int i = 0; i < length; i++)
            {
                values[i] = reader.ReadUInt16();
            }

            return values;
        }

        /// <summary>
        /// Converts a value that represents BGR565 
        /// </summary>
        /// <param name="value">The BGR565 value (stored as a UInt16) to convert</param>
        /// <returns>Returns the equivalent RGB32 of the BGR565 value</returns>
        protected uint ConvertBGR565ToUInt32(ushort value)
        {
            byte r5 = (byte)((value >> 11) & 0x1f);
            byte g6 = (byte)((value >> 5) & 0x3f);
            byte b5 = (byte)(value & 0x1f);

            // Map from a 5bit value (0-31) or 6bit value (0-63) to 8bit value (0-255).
            byte r8 = (byte)((r5 << 3) + ((r5 << 3) >> 5));
            byte g8 = (byte)((g6 << 2) + ((g6 << 2) >> 6));
            byte b8 = (byte)((b5 << 3) + ((b5 << 3) >> 5));
            byte a8 = 0xFF;

            return (uint)(r8 | g8 << 8 | b8 << 16 | a8 << 24);
        }

        /// <summary>
        /// Converts a value that represents BGRA5551
        /// </summary>
        /// <param name="value">The BGRA5551 value (stored as a UInt16) to convert</param>
        /// <returns>Returns the equivalent RGB32 of the BGRA5551 value</returns>
        protected uint ConvertBGRA5551ToUInt32(ushort value)
        {
            byte a1 = (byte)((value >> 15) & 0x01);
            byte r5 = (byte)((value >> 10) & 0x1f);
            byte g5 = (byte)((value >> 5) & 0x1f);
            byte b5 = (byte)(value & 0x1f);

            // Map from a 5bit value (0-31) to 8bit value (0-255).
            byte r8 = (byte)((r5 << 3) + ((r5 << 3) >> 5));
            byte g8 = (byte)((g5 << 3) + ((g5 << 3) >> 5));
            byte b8 = (byte)((b5 << 3) + ((b5 << 3) >> 5));
            byte a8 = a1 == 0x01 ? (byte)0xFF : (byte)0x00;

            return (uint)(r8 | g8 << 8 | b8 << 16 | a8 << 24);
        }

        /// <summary>
        /// Converts a value that represents BGRA4444 
        /// </summary>
        /// <param name="value">The BGRA4444 value (stored as a UInt16) to convert</param>
        /// <returns>Returns the equivalent RGB32 of the BGRA4444 value</returns>
        protected uint ConvertBGRA4444ToUInt32(ushort value)
        {
            byte a4 = (byte)((value >> 12) & 0x0f);
            byte r4 = (byte)((value >> 8)& 0x0f);
            byte g4 = (byte)((value >> 4) & 0x0f);
            byte b4 = (byte)(value & 0x0f);

            // Map from a 4bit value (0-15) to 8bit value (0-255).
            byte r8 = (byte)(r4 << 0x04);
            byte g8 = (byte)(g4 << 0x04);
            byte b8 = (byte)(b4 << 0x04);
            byte a8 = (byte)(a4 << 0x04);

            return (uint)(r8 | g8 << 8 | b8 << 16 | a8 << 24);
        }

        /// <summary>
        /// Converts a UInt16 to an image pixel (RGBA as a UInt32)
        /// </summary>
        /// <param name="source">The UInt16 value to convert to an image pixel</param>
        /// <param name="format">The format of the source</param>
        /// <returns>A UInt32 value that is the equivalent of the source provided</returns>
        protected uint ConvertToImagePixel(ushort source, ImageFormat format)
        {
            switch (format)
            {
                case ImageFormat.BGR565: return ConvertBGR565ToUInt32(source);
                case ImageFormat.BGRA5551: return ConvertBGRA5551ToUInt32(source);
                case ImageFormat.BGRA4444: return ConvertBGRA4444ToUInt32(source);
                default:
                    return 0;
            }
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

        /// <summary>
        /// Returns the image data located at a location in the file
        /// </summary>
        /// <param name="offset">The starting position to read the data</param>
        /// <returns>An instance of <see cref="Image" />.  If there is no valid image data, null is returned.</returns>
        public Image ReadImage(long offset)
        {
            //First grab the first 40 bytes (as UInt32).  This is the header record.
            _reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            uint[] headerData = ReadAsUInt32Array(_reader, 10);

            //If the first value is not 0x00008005, then it is not a valid image
            if (headerData[0] != 0x00008005)
                return null;

            //Get the width and height as that's needed to determine how to read the next values
            int width = (int)headerData[4];
            int height = (int)headerData[5];
            if (width < 1 || width > 1024
                || height < 1 || height > 1024)
                return null;

            ImageFormat format = (ImageFormat)headerData[9];
            uint[] pixels = ReadAsUInt16Array(_reader, width * height)
                                .Select(pixel => ConvertToImagePixel(pixel, format))
                                .ToArray();

            return new Image()
            {
                FileType = headerData[0],
                UnknownValue1 = headerData[1],
                UnknownValue2 = headerData[2],
                UnknownValue3 = headerData[3],
                Width = width,
                Height = height,
                MipmappingLevel = headerData[6],
                UnknownValue4 = headerData[7],
                UnknownValue5 = headerData[8],
                Format = format,
                Data = pixels
            };
        }

    }
}