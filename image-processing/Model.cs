using System;
using System.Windows.Forms;
using System.Drawing;

namespace image_processing
{
    class Model
    {
        public Form View { get; }

        public Model()
        {
            View = new MainForm(this);
        }

        public double CalculatePSNR(Image a, Image b)
        {
            var abmp = new Bitmap(a);
            var bbmp = new Bitmap(b);
            var sum = 0l;
            for (var i = 0; i < 512; i++)
            {
                for (var j = 0; j < 512; j++)
                {
                    sum += (int)Math.Pow(abmp.GetPixel(i, j).R - bbmp.GetPixel(i, j).R, 2);
                    sum += (int)Math.Pow(abmp.GetPixel(i, j).G - bbmp.GetPixel(i, j).G, 2);
                    sum += (int)Math.Pow(abmp.GetPixel(i, j).B - bbmp.GetPixel(i, j).B, 2);
                }
            }
            double q = 3 * 255 * 255 * 512.0 / sum * 512;
            return 10 * Math.Log10(q);
        }
    }
}
