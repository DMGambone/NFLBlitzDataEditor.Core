namespace NFLBlitzDataEditor.Core.Imaging
{
	/// <summary>
	/// 
	/// </summary>
	public class RGB332ColorFormat
		: IColorFormat
	{
		/// <summary>
		/// A mapping of 8-bit encoded color to 32-bit encoded color
		/// </summary>
		private readonly uint[] _rgb332MappingTable;

		private readonly ushort[] _rgb3bitTable = new ushort[] { 0, 36, 72, 108, 144, 180, 216, 255 };

		private readonly ushort[] _rgb2bitTable = new ushort[] { 0, 85, 170, 255 };

		public RGB332ColorFormat()
		{
			_rgb332MappingTable = GenerateRGB332Table();
		}

		public virtual uint ToRGBA(ushort color)
		{
			if(color > 255)
				throw new System.ArgumentOutOfRangeException(nameof(color), $"Expected a value between 0 and 255, got {color}");

			return _rgb332MappingTable[color];
		}

		/// <summary>
		/// Generates a mapping table of 8-bit 332 color encoding to 32-bit color
		/// </summary>
		/// <returns>A collection of uint of the mapped colors</returns>
		private uint[] GenerateRGB332Table()
		{
			uint[] mappingTable = new uint[256];
			for (int index = 0; index < mappingTable.Length; index++)
			{
				// Split the index into 3-bit red, 3-bit green, 2-bit blue
				byte r = (byte)((index & 0xE0) >> 5);
				byte g = (byte)((index & 0x1C) >> 2);
				byte b = (byte)(index & 0x03);

				mappingTable[index] = (uint)((_rgb3bitTable[r] << 16) | (_rgb3bitTable[g] << 8) | _rgb2bitTable[b] | 0xff000000);
			}

			return mappingTable;
		}
	}
}