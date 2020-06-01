using System;
using System.Linq;
using System.IO;
using NFLBlitzDataEditor.Core.Models;
using NFLBlitzDataEditor.Core.Enums;
using NFLBlitzDataEditor.Core.Extensions;

namespace NFLBlitzDataEditor.Core.Readers
{
    /// <summary>
    /// Used to read an image from a stream of data
    /// </summary>
    public class ImageDataReader
        : IDataFileRecordReader<ImageData>
    {

        private readonly uint[] _rgb332MappingTable;

        public ImageDataReader()
        {
            _rgb332MappingTable = GetRGB332Table();
        }

        private uint[] GetRGB332Table()
        {
            uint[] mappingTable = new uint[256];
            byte r8 = 0;
            byte index = 0;
            for (byte r = 0; r < 3; r++)
            {
                byte g8 = 0;
                for (byte g = 0; g < 3; g++)
                {
                    g8 = (byte)((g8 << 1) + 1);

                    byte b8 = 0;
                    for (byte b = 0; b < 2; b++)
                    {

                        b8 = (byte)((b8 << 1) + 1);

                        uint color = (uint)(r8 | (g8 << 8) | (b8 << 8) | 0xff000000);
                        mappingTable[index] = color;
                        b8 += 127;

                    }
                }

                r8 = (byte)((r8 << 1) + 1);
            }

            return mappingTable;
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
            byte r4 = (byte)((value >> 8) & 0x0f);
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
        /// Converts an RGB332 value to 32bit color
        /// </summary>
        /// <param name="value">The RGB332 value to convert</param>
        /// <returns>Returns the equivalent RGB32 of the indexed value</returns>
        protected uint ConvertRGB332ToUInt32(byte value)
        {
            return _rgb332MappingTable[value];
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
                case ImageFormat.RGB332: return ConvertRGB332ToUInt32((byte)(source & 0xFF));
                case ImageFormat.RGB565: return ConvertBGR565ToUInt32(source);
                case ImageFormat.ARGB1555: return ConvertBGRA5551ToUInt32(source);
                case ImageFormat.ARGB4444: return ConvertBGRA4444ToUInt32(source);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Reads a sequence of bytes that represents the image pixel data
        /// </summary>
        /// <param name="format">The format of the image data</param>
        /// <param name="width">The image width</param>
        /// <param name="height">The image height</param>
        /// <returns></returns>
        protected uint[] ReadImageData(ImageFormat format, int width, int height, BinaryReader reader)
        {
            if (format == ImageFormat.RGB332)
            {
                // The data is stored as bytes
                return reader.ReadBytes(width * height)
                            .Select(pixel => ConvertToImagePixel((ushort)pixel, format))
                            .ToArray();
            }
            else
            {
                // The data is stored as UInt16 values
                return reader.ReadAsUInt16Array(width * height)
                                .Select(pixel => ConvertToImagePixel(pixel, format))
                                .ToArray();
            }
        }

        public ImageData Read(BinaryReader reader)
        {
            //Read in the whole image header
            uint[] headerData = reader.ReadAsUInt32Array(10);

            //If the first value is not 0x00008005, then it is not a valid image
            if (headerData[0] != 0x00008005)
                return null;

            //Get the width and height as that's needed to determine how to read the next values
            int width = (int)headerData[4];
            int height = (int)headerData[5];
            if (width < 1 || width > 256
                || height < 1 || height > 256)
                return null;

            if (!Enum.IsDefined(typeof(ImageFormat), (int)headerData[9]))
                return null;

            ImageFormat format = (ImageFormat)headerData[9];
            uint[] pixels = ReadImageData(format, width, height, reader);

            return new ImageData()
            {
                Version = headerData[0],
                Bias = headerData[1],
                FilterMode = (MipMapFilterMode)headerData[2],
                UseTrilinearFiltering = headerData[3] != 0,
                Width = width,
                Height = height,
                SmallestLOD = (LevelOfDetail)headerData[6],
                LargestLOD = (LevelOfDetail)headerData[7],
                AspectRatio = (LODAspectRatio)headerData[8],
                Format = format,
                Data = pixels
            };
        }

    }
}

