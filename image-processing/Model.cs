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
            using (var fastBitmapA = (new Bitmap(a).FastLock()))
            {
                using (var fastBitmapB = (new Bitmap(b).FastLock()))
                {
                    var sum = 0l;
                    for (var i = 0; i < 512; i++)
                    {
                        for (var j = 0; j < 512; j++)
                        {
                            sum += (int)Math.Pow(fastBitmapA.GetPixel(i, j).R - fastBitmapB.GetPixel(i, j).R, 2);
                            sum += (int)Math.Pow(fastBitmapA.GetPixel(i, j).G - fastBitmapB.GetPixel(i, j).G, 2);
                            sum += (int)Math.Pow(fastBitmapA.GetPixel(i, j).B - fastBitmapB.GetPixel(i, j).B, 2);
                        }
                    }
                    return 10 * Math.Log10(255 * 255 * 512.0 / sum * 512);
                }
            }
        }
    }
}
