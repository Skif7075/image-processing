using System;
using System.Drawing;

namespace image_processing.Extensions
{
    static class BitmapExtensions
    {
        public static void ToGrayscale(this Bitmap bmp, Func<Color, byte> grayscaleFunction)
        {
            using (var fastBitmap = bmp.FastLock())
            {
                for (var y = 0; y < fastBitmap.Height; y++)
                {
                    for (var x = 0; x < fastBitmap.Width; x++)
                    {
                        var value = grayscaleFunction(fastBitmap.GetPixel(x,y));
                        fastBitmap.SetPixel(x, y, Color.FromArgb(value, value, value));
                    }
                }
            }
        }
        public static FastBitmap FastLock(this Bitmap bitmap)
        {
            FastBitmap fast = new FastBitmap(bitmap);
            fast.Lock();

            return fast;
        }
    }
}
