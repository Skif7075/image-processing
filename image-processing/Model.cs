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
    }
}
