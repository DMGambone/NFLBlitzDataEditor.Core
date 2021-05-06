namespace NFLBlitzDataEditor.Core.Imaging
{
	public interface IColorFormat
	{
		/// <summary>
		/// Converts a color value into a full RGBA value
		/// </summary>
		/// <param name="color">The native color value to covert</param>
		/// <returns>A 32-bit RGBA color</returns>
		uint ToRGBA(ushort color);
	}
}