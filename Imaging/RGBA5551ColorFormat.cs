namespace NFLBlitzDataEditor.Core.Imaging
{
	public class RGBA5551ColorFormat
		: IColorFormat
	{
		public uint ToRGBA(ushort color)
		{
			byte a1 = (byte)((color >> 15) & 0x01);
			byte r5 = (byte)((color >> 10) & 0x1f);
			byte g5 = (byte)((color >> 5) & 0x1f);
			byte b5 = (byte)(color & 0x1f);

			// Map from a 5bit value (0-31) to 8bit value (0-255).
			byte r8 = (byte)((r5 << 3) + ((r5 << 3) >> 5));
			byte g8 = (byte)((g5 << 3) + ((g5 << 3) >> 5));
			byte b8 = (byte)((b5 << 3) + ((b5 << 3) >> 5));
			byte a8 = a1 == 0x01 ? (byte)0xFF : (byte)0x00;

			return (uint)(r8 | g8 << 8 | b8 << 16 | a8 << 24);		}
	}
}