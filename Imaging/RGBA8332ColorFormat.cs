namespace NFLBlitzDataEditor.Core.Imaging
{
    public class RGBA8332ColorFormat
		: RGB332ColorFormat
    {
		public override uint ToRGBA(ushort color)
		{
			// Get the 332 color
			uint rgb =  base.ToRGBA((ushort)(color & 0x00ff));

			// Merge the alpha color into the 332 color
			return (uint)((color & 0xff00) << 16) | rgb;
		}

	
	
    }
}