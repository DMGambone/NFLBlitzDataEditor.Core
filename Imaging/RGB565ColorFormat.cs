namespace NFLBlitzDataEditor.Core.Imaging
{
	/// <summary>
	/// BGR565 Color Format
	/// </summary>
	public class RGB565ColorFormat
		: IColorFormat
	{
		public uint ToRGBA(ushort color)
		{
			byte r5 = (byte)((color >> 11) & 0x1f);
			byte g6 = (byte)((color >> 5) & 0x3f);
			byte b5 = (byte)(color & 0x1f);

			// Map from a 5bit value (0-31) or 6bit value (0-63) to 8bit value (0-255).
			byte r8 = (byte)((r5 << 3) + ((r5 << 3) >> 5));
			byte g8 = (byte)((g6 << 2) + ((g6 << 2) >> 6));
			byte b8 = (byte)((b5 << 3) + ((b5 << 3) >> 5));
			byte a8 = 0xFF;

			return (uint)(r8 | g8 << 8 | b8 << 16 | a8 << 24);
		}
	}
}