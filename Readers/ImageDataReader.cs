using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NFLBlitzDataEditor.Core.Models;
using NFLBlitzDataEditor.Core.Enums;
using NFLBlitzDataEditor.Core.Imaging;
using NFLBlitzDataEditor.Core.Extensions;

namespace NFLBlitzDataEditor.Core.Readers
{

	/// <summary>
	/// Used to read an image from a stream of data
	/// </summary>
	public class ImageDataReader
	{
		public readonly static IDictionary<ImageFormat, IColorFormat> StandardColorFormats = new Dictionary<ImageFormat, IColorFormat>()
		{
				{ ImageFormat.RGB332, new RGB332ColorFormat() },
				{ ImageFormat.ARGB8332, new RGBA8332ColorFormat() },
				{ ImageFormat.RGB565, new RGB565ColorFormat() },
				{ ImageFormat.ARGB1555, new RGBA5551ColorFormat() },
				{ ImageFormat.ARGB4444, new RGBA4444ColorFormat() }
		};

		/// <summary>
		/// The numeric identifier all images start with
		/// </summary>
		private const uint IMAGE_IDENTIFIER = 0x00008005;

		/// <summary>
		/// Color format converters used to change the native color value into RGBA values
		/// </summary>
		/// <returns></returns>
		private readonly IDictionary<ImageFormat, IColorFormat> _colorFormats;

		/// <summary>
		/// 
		/// </summary>
		public ImageDataReader()
			: this(ImageDataReader.StandardColorFormats)
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="colorFormats">The color formatters to use</param>
		public ImageDataReader(IDictionary<ImageFormat, IColorFormat> colorFormats)
		{
			_colorFormats = colorFormats;
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
			if(!_colorFormats.ContainsKey(format))
				throw new InvalidOperationException($"{format} is currently not supported");
			IColorFormat colorFormat = _colorFormats[format];

			if (format == ImageFormat.RGB332)
			{
				// The data is stored as bytes
				return reader.ReadBytes(width * height)
							.Select(pixel => colorFormat.ToRGBA((ushort)pixel))
							.ToArray();
			}
			else
			{
				// The data is stored as UInt16 values
				return reader.ReadAsUInt16Array(width * height)
								.Select(pixel => colorFormat.ToRGBA(pixel))
								.ToArray();
			}
		}

		/// <summary>
		/// Reads data from a <see cref="BinaryReader" /> into ImageData
		/// </summary>
		/// <param name="reader">An instance of a <see cref="BinaryReader" /> to read the data from</param>
		/// <returns>An instance of <see cref="ImageData" /> representing the data.  If the data is not a valid format, null is returned</returns>
		/// <exception cref="InvalidDataException">Thrown when the reader does not contain valid image data</exception>
		public ImageData Read(BinaryReader reader)
		{
			//Read in the whole image header
			uint[] headerData = reader.ReadAsUInt32Array(10);

			//If the first value is not 0x00008005, then it is not a valid image
			if (headerData[0] != 0x00008005)
				throw new InvalidDataException($"Expected a key of ${ImageDataReader.IMAGE_IDENTIFIER}, but got {headerData[0]}");

			//Get the width and height as that's needed to determine how to read the next values
			int width = (int)headerData[4];
			int height = (int)headerData[5];

			if (!Enum.IsDefined(typeof(ImageFormat), (int)headerData[9]))
				throw new InvalidDataException($"Unsupported image format {headerData[9]} provided");

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

