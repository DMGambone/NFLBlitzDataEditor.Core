using System;
using System.IO;

namespace NFLBlitzDataEditor.Core.Extensions
{
    public static class BinaryReaderExtensions
    {
        /// <summary>
        /// Swaps the bytes in a 32-bit word
        /// </summary>
        /// <param name="word">The 32-bit word to swap the values for</param>
        /// <returns>A value of <paramref name="word" /> with the endianness reversed</returns>
        private static byte[] SwapBytes(byte[] word)
        {
            return new byte[4] { word[3], word[2], word[1], word[0] };
        }

        /// <summary>
        /// Returns a reader used to read a single block of data from another stream
        /// </summary>
        /// <param name="reader">The reader to read the bytes from</param>
        /// <param name="recordSize">The size of the team data block</param>
        /// <returns>An instance of a BinaryReader that can be used to read the block of data.</returns>
        public static BinaryReader ReadNextBlock(this BinaryReader reader, int recordSize)
        {
            byte[] block = reader.ReadBytes(recordSize);

            return new BinaryReader(new MemoryStream(block));
        }

        /// <summary>
        /// Reads the next set of bytes and converts it into a string
        /// </summary>
        /// <param name="reader">The reader to read the bytes from</param>
        /// <param name="length">The length of the string to read.  -1 indicates if the read should keep reading until the first null terminated is encountered</param>
        /// <param name="bigEndian">Indicates that the string is stored using big-endian encoding.</param>
        /// <returns>A string containing the data that was read.  Any trailing null characters are trimmed.</returns>
        public static string ReadAsString(this BinaryReader reader, int length = -1, bool bigEndian = false)
        {
            string value = "";

            for (int idx = 0; idx < length || length == -1; idx += 4)
            {
                //Read 4 bytes
                byte[] bytes = reader.ReadBytes(4);

                if (bigEndian)
                    bytes = SwapBytes(bytes);

                value += System.Text.Encoding.ASCII.GetString(bytes).Trim((char)0);

                //If we're looking for null-terminated strings of variable size, then break if what was just added to the value
                // was 4 characters.
                if(length == -1 
                    && value.Length % 4 != 0)
                    break;
            }

            return value;
        }

        /// <summary>
        /// Reads the next value as a Date/Time value 
        /// </summary>
        /// <param name="reader">The reader to read the int from</param>
        public static DateTime ReadTimestamp(this BinaryReader reader)
        {
            //A timestamp is stored in 32 bits, with the time in the first 16 bits and date in the second 16 bits.
            ushort time = reader.ReadUInt16();
            ushort date = reader.ReadUInt16();

            //Time: hours, minutes, seconds are stored as 5 bits, 6 bits, 5 bits (respectively)
            int hour = (time >> 11) & 0x1f;
            int minute = (time >> 5) & 0x3f;
            int second = (time & 0x1f) * 2;

            //Date: year (since 1980), month, day are stored as 7 bits, 4 bits, 5 bits (respectively)
            int year = (date >> 9) & 0x5f;
            int month = (date >> 5) & 0x0f;
            int day = date & 0x1f;

            if (year == 0 || month == 0 || day == 0)
                return DateTime.MinValue;

            return new DateTime(1980 + year, month, day, hour, minute, second);
        }

        /// <summary>
        /// Reads an array of int values
        /// </summary>
        /// <param name="reader">The reader to read the int from</param>
        /// <param name="length">The number of int values to read</param>
        /// <returns>An array of int values.</returns>
        public static int[] ReadAsInt32Array(this BinaryReader reader, int length)
        {
            int[] values = new int[length];
            for (int i = 0; i < length; i++)
            {
                values[i] = reader.ReadInt32();
            }

            return values;
        }

        /// <summary>
        /// Reads an array of UInt16 values
        /// </summary>
        /// <param name="reader">The reader to read the UInt16 from</param>
        /// <param name="length">The number of values to read</param>
        /// <returns>An array of unsigned UInt16 values.</returns>
        public static ushort[] ReadAsUInt16Array(this BinaryReader reader, int length)
        {
            ushort[] values = new ushort[length];
            for (int i = 0; i < length; i++)
            {
                values[i] = reader.ReadUInt16();
            }

            return values;
        }



        /// <summary>
        /// Reads an array of unsigned int values
        /// </summary>
        /// <param name="reader">The reader to read the int from</param>
        /// <param name="length">The number of values to read</param>
        /// <returns>An array of unsigned int values.</returns>
        public static uint[] ReadAsUInt32Array(this BinaryReader reader, int length)
        {
            uint[] values = new uint[length];
            for (int i = 0; i < length; i++)
            {
                values[i] = reader.ReadUInt32();
            }

            return values;
        }

    }
}