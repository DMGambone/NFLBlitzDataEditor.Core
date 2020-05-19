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
    public class ImageRecordReader
        : IDataFileRecordReader<Image>
    {
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
        /// Converts an alpha value to 32bit color
        /// </summary>
        /// <param name="value">The alpha value to convert</param>
        /// <returns>Returns the equivalent RGB32 of the indexed value</returns>
        protected uint ConvertBitmapMaskToUInt32(byte source)
        {
            return (uint)(source << 24);
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
                case ImageFormat.BitmapMask: return ConvertBitmapMaskToUInt32((byte)(source & 0xFF));
                case ImageFormat.BGR565: return ConvertBGR565ToUInt32(source);
                case ImageFormat.BGRA5551: return ConvertBGRA5551ToUInt32(source);
                case ImageFormat.BGRA4444: return ConvertBGRA4444ToUInt32(source);
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
            if (format == ImageFormat.BitmapMask)
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

        public Image Read(BinaryReader reader)
        {
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
