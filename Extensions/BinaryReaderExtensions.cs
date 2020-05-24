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
        /// <param name="length">The length of the string to read</param>
        /// <returns>A string containing the data that was read.  Any trailing null characters are trimmed.</returns>
        public static string ReadAsString(this BinaryReader reader, int length)
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
        /// Reads the next set of bytes until it reaches a null character
        /// </summary>
        /// <param name="reader">The reader to read the bytes from</param>
        /// <param name="quadBoundedString">Indicates if reading in the string should be done as a single character (false), or as four characters (true)</param>
        /// <returns>A string containing the data that was read.</returns>
        public static string ReadAsString(this BinaryReader reader, bool quadBoundedString = false)
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            int readSize = (quadBoundedString ? 4 : 1);

            while (reader.PeekChar() != -1)
            {
                char nextCharacter = (char)0;

                //Either read 1 character of 4 characters
                for (int i = 0; i < readSize; i++)
                {
                    nextCharacter = reader.ReadChar();
                    if (nextCharacter != 0x00)
                        stringBuilder.Append(nextCharacter);
                }

                //Was the last character read a null value?
                if (nextCharacter == 0x00)
                    break;
            }

            return stringBuilder.ToString();
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