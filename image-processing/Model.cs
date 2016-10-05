using System;
using System.Windows.Forms;
using System.Drawing;
using image_processing.Extensions;

namespace image_processing
{
    class Model : IObserver
    {
        public MainForm View { get; }

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
                    var sum = 0L;
                    for (var i = 0; i < 512; i++)
                    {
                        for (var j = 0; j < 512; j++)
                        {
                            sum += (int)Math.Pow(fastBitmapA.GetPixel(i, j).R - fastBitmapB.GetPixel(i, j).R, 2);
                            sum += (int)Math.Pow(fastBitmapA.GetPixel(i, j).G - fastBitmapB.GetPixel(i, j).G, 2);
                            sum += (int)Math.Pow(fastBitmapA.GetPixel(i, j).B - fastBitmapB.GetPixel(i, j).B, 2);
                        }
                    }
                    var psnr = 10 * Math.Log10((3 << 18) / ((double)sum) * 255 * 255);
                    if (psnr < 10e-8)
                        return 0;
                    return psnr;
                }
            }
        }

        public void Update(IImageChangeObservable pctureHolder)
        {
            if (!View.LeftPictureWrapper.IsEmpty()&&!View.RightPictureWrapper.IsEmpty())
                View.PsnrLabel.Text = CalculatePSNR(View.LeftPictureWrapper.Image,View.RightPictureWrapper.Image).ToString();
        }
    }
}
