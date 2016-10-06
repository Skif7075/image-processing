using System;
using System.Drawing;
using image_processing.Extensions;

namespace image_processing
{
    public struct YCbCr
    {
        public byte Y;
        public byte Cb;
        public byte Cr;

        static private byte NormalizeByte(int num)
        {
            return (byte)Math.Min(Math.Max(num, 0),255);
        }
        static public byte ToY(Color rgbColor)
        {
            return NormalizeByte((int)Math.Round((77 * rgbColor.R + 150 * rgbColor.G + 29 * rgbColor.B) / 256.0));
        }
        static public byte ToCb(Color rgbColor)
        {
            return NormalizeByte((int)Math.Round((-43 * rgbColor.R - 85 * rgbColor.G + 128 * rgbColor.B) / 256.0 + 128));
        }
        static public byte ToCb(Color rgbColor, byte y)
        {
            return NormalizeByte((int)Math.Round((144.0 / 256 * (rgbColor.B - y) + 128)));
        }
        static public byte ToCr(Color rgbColor)
        {
            return NormalizeByte((int)Math.Round((128 * rgbColor.R - 107 * rgbColor.G - 21 * rgbColor.B) / 256.0 + 128));
        }
        static public byte ToCr(Color rgbColor, byte y)
        {
            return NormalizeByte((int)Math.Round((183.0 / 256) * (rgbColor.R - y) + 128));
        }
        static public byte ToR(YCbCr ycbcr)
        {
            return NormalizeByte((int)Math.Round(ycbcr.Y + (256.0 / 183) * (ycbcr.Cr - 128)));
        }
        static public byte ToG(YCbCr ycbcr)
        {
            return NormalizeByte((int)Math.Round(ycbcr.Y - (5329.0 / 15481) * (ycbcr.Cb - 128) - (11103.0 / 15481) * (ycbcr.Cr - 128)));
        }
        static public byte ToB(YCbCr ycbcr)
        {
            return NormalizeByte((int)Math.Round(ycbcr.Y + (256.0 / 144) * (ycbcr.Cb - 128)));
        }
        static public YCbCr ToYcbcr(Color rgbColor)
        {
            var y = ToY(rgbColor);
            return new YCbCr { Y = y, Cb = ToCb(rgbColor, y), Cr = ToCr(rgbColor, y) };
        }
        static public Color ToRgb(YCbCr ycbcr)
        {
            return Color.FromArgb(ToR(ycbcr), ToG(ycbcr), ToB(ycbcr));
        }
        static public YCbCr[,] ToYcbcr(Bitmap bmp)
        {
            var ycbcrBitmap = new YCbCr[512, 512];
            using (var fastBitmap = bmp.FastLock())
            {
                for (var y = 0; y < 512; y++)
                {
                    for (var x = 0; x < 512; x++)
                    {
                        ycbcrBitmap[x,y] = ToYcbcr(fastBitmap.GetPixel(x, y));
                    }
                }
            }
            return ycbcrBitmap;
        }
        static public Bitmap ToRgb(YCbCr[,] ycbcrBitmap)
        {
            var bmp = new Bitmap(512, 512);
            using (var fastBitmap = bmp.FastLock())
            {
                for (var y = 0; y < 512; y++)
                {
                    for (var x = 0; x < 512; x++)
                    {
                        fastBitmap.SetPixel(x, y, ToRgb(ycbcrBitmap[x, y]));
                    }
                }
            }
            return bmp;
        }
    }
}
