using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace image_processing
{
    class PictureBoxWrapper : Control, IImageChangeObservable
    {
        private List<IObserver> observers = new List<IObserver>();
        private PictureBox pictureBox;
        public Image Image
        {
            set
            {
                pictureBox.Image = value;
                NotifyObservers();
            }
            get { return pictureBox.Image; }
        }
        public PictureBoxWrapper(PictureBox pictureBox)
        {
            Controls.Add(pictureBox);
            this.pictureBox = pictureBox;
            pictureBox.Click += (sender,args) => OnClick(args);
            Width = pictureBox.Width;
            Height = pictureBox.Height;
        }
        public bool IsEmpty()
        {
            return (Image == null);
        }

        public void RegisterObserver(IObserver observer)
        {
            observers.Add(observer);
        }

        public void NotifyObservers()
        {
            foreach(var observer in observers)
            {
                observer.Update(this);
            }
        }

    }
}
