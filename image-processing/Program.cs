using System;
using System.Windows.Forms;

namespace image_processing
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var model = new Model();
            Application.Run(model.View);
        }
    }
}
