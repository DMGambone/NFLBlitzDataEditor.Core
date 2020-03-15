using System;
using System.Linq;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;

namespace NFLBlitz2kDataEditor.Utilities
{
    public static class ImageConverter
    {
        public static void SaveAsPNG(NFLBlitz2kDataEditor.Models.Image image, string destPath)
        {
            Rgba32[] pixels = image.Data.Select(pixel => new Rgba32(pixel)).ToArray();
            Image outputImage = Image.LoadPixelData<Rgba32>(pixels, image.Width, image.Height);

            using (System.IO.FileStream outputStream = System.IO.File.OpenWrite(destPath))
            {
                PngEncoder encoder = new PngEncoder();
                outputImage.Save(outputStream, encoder);
            }
        }
    }
}