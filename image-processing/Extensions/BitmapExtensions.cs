using System;
using System.Drawing;

namespace image_processing.Extensions
{
    static class BitmapExtensions
    {
        public static Bitmap ToGrayscale(this Bitmap bmp, Func<Color, byte> grayscaleFunction)
        {
            using (var dbmp = new DirectBitmap(bmp.Width, bmp.Height))
            {
                for (var y = 0; y < dbmp.Height; y++)
                {
                    for (var x = 0; x < dbmp.Width; x++)
                    {
                        var color = bmp.GetPixel(x, y);
                        var value = grayscaleFunction(color);
                        dbmp[x, y] = Color.FromArgb(value, value, value);
                    }
                }
                return (Bitmap)dbmp.Bitmap.Clone();
            }
        }
    }
}
