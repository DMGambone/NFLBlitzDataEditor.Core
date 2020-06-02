using System;
using System.IO;

namespace NFLBlitzDataEditor.Core.Extensions
{
    public static class BinaryReaderExtensions
    {

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
        /// <param name="maxLength">The maximum length of the string to read.  -1 indicates if the read should keep reading until the first null terminated is encountered</param>
        /// <param name="bigEndian">Indicates that the string is stored using big-endian encoding.</param>
        /// <returns>A string containing the data that was read up to the first null character.</returns>
        public static string ReadAsString(this BinaryReader reader, int maxLength = -1)
        {
            string value = "";

            for (int idx = 0; idx < maxLength; idx++)
            {
                char nextChar = reader.ReadChar();
                if (nextChar == (char)0)
                {
                    //Consume the rest of the string and toss it away
                    reader.ReadBytes(maxLength - idx - 1);
                    break;
                }

                value += nextChar;
            }

            return value;
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