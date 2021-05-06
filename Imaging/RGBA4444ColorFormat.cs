namespace NFLBlitzDataEditor.Core.Imaging
{
	public class RGBA4444ColorFormat
		: IColorFormat
	{
		public uint ToRGBA(ushort color)
		{
			byte a4 = (byte)((color >> 12) & 0x0f);
			byte r4 = (byte)((color >> 8) & 0x0f);
			byte g4 = (byte)((color >> 4) & 0x0f);
			byte b4 = (byte)(color & 0x0f);

			// Map from a 4bit value (0-15) to 8bit value (0-255).
			byte r8 = (byte)(r4 << 0x04);
			byte g8 = (byte)(g4 << 0x04);
			byte b8 = (byte)(b4 << 0x04);
			byte a8 = (byte)(a4 << 0x04);

			return (uint)(r8 | g8 << 8 | b8 << 16 | a8 << 24);		}
	}
}