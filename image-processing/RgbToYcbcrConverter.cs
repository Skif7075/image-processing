using System;
using System.Drawing;
using image_processing.Extensions;

namespace image_processing
{
    static class RgbToYcbcrConverter
    {
        static public byte GetY (Color rgbColor)
        {
            return (byte)Math.Round((77 * rgbColor.R + 150 * rgbColor.G + 29 * rgbColor.B)/256.0);
        }
        static public byte GetCb(Color rgbColor)
        {
            return (byte)Math.Round((-43 * rgbColor.R - 85 * rgbColor.G + 128 * rgbColor.B) / 256.0 + 128);
        }
        static public byte GetCb(Color rgbColor, byte y)
        {
            return (byte)Math.Round((144.0/256 * (rgbColor.B - y) + 128));
        }
        static public byte GetCr(Color rgbColor)
        {
            return (byte)Math.Round((128 * rgbColor.R - 107 * rgbColor.G - 21 * rgbColor.B) / 256.0 + 128);
        }
        static public byte GetCr(Color rgbColor, byte y)
        {
            return (byte)Math.Round((183.0/256) * (rgbColor.R - y) + 128);
        }
        
        static public YCbCr ToYcbcr (Color rgbColor)
        {
            var y = GetY(rgbColor);
            return new YCbCr { Y = y, Cb = GetCb(rgbColor, y), Cr = GetCr(rgbColor, y) };
        }
        static public Color ToRgb(YCbCr ycbcr)
        {
            var r = (int)(ycbcr.Y + (256.0 / 183) * (ycbcr.Cr - 128));
            var g = (int)(ycbcr.Y - (5329.0 / 15481) * (ycbcr.Cb - 128) - (11103.0/15481)*(ycbcr.Cr - 128));
            var b = (int)(ycbcr.Y + (256.0 / 144) * (ycbcr.Cb - 128));
            r = Math.Max(r, 0);
            g = Math.Max(g, 0);
            b = Math.Max(b, 0);
            return Color.FromArgb(r, g, b);
        }

        static public Bitmap ToYcbcr(Bitmap bmp)
        {
            var newBmp = (Bitmap)bmp.Clone();
            using (var fastBitmap = newBmp.FastLock())
            {
                for (var y = 0; y < 512; y++)
                {
                    for (var x = 0; x < 512; x++)
                    {
                        var ycbcr = ToYcbcr(fastBitmap.GetPixel(x, y));
                        fastBitmap.SetPixel(x, y, Color.FromArgb(ycbcr.Y, ycbcr.Cb, ycbcr.Cr)); 
                    }
                }
            }
            return newBmp;
        }
        static public Bitmap ToYcbcrToRgb(Bitmap bmp)
        {
            var newBmp = (Bitmap)bmp.Clone();
            using (var fastBitmap = newBmp.FastLock())
            {
                for (var y = 0; y < 512; y++)
                {
                    for (var x = 0; x < 512; x++)
                    {
                        var ycbcr = ToYcbcr(fastBitmap.GetPixel(x, y));
                        var rgb = ToRgb(ycbcr);
                        fastBitmap.SetPixel(x, y, rgb);
                    }
                }
            }
            return newBmp;
        }
    }
}
