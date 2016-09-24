using System;
using System.Drawing;

namespace image_processing
{
    class ColorInfo
    {
        public static Func<Color, byte> AvgComponent = delegate (Color color)
        { return (byte)Math.Round((color.R + color.G + color.B) / 3.0); };

        public static Func<Color, byte> Luminance = delegate (Color color)
        { return (byte)Math.Round((color.R*299 + color.G*587 + color.B*114) / 1000.0); };
    }
}
