using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using image_processing.Extensions;

namespace image_processing
{
    class MainForm : Form
    {
        public PictureBoxWrapper LeftPictureWrapper { get; }
        public PictureBoxWrapper RightPictureWrapper { get; }
        public Label PsnrLabel { get; }
        private Model Model { get; }

        public MainForm(Model model)
        {
            Model = model;
            Width = 1100;
            Height = 700;

            //Context Menu
            var pictureBoxContextMenu = new ContextMenuStrip();

            var itemNames = new string[] {"Save","Duplicate","Avg","Lum"};
            foreach (var itemName in itemNames)
            {
                var item = new ToolStripMenuItem(itemName);
                pictureBoxContextMenu.Items.Add(item);
                item.Click += new EventHandler(ContextMenuItem_Click);
            }

            //PictureBoxes
            var leftPictureBox = new PictureBox()
            {
                BackColor = Color.White,
                Height = 512,
                Width = 512
            };
            LeftPictureWrapper = new PictureBoxWrapper(leftPictureBox);
            LeftPictureWrapper.Click += new EventHandler(LoadImage);
            LeftPictureWrapper.ContextMenuStrip = pictureBoxContextMenu;
            LeftPictureWrapper.RegisterObserver(model);

            var rightPictureBox = new PictureBox()
            {
                BackColor = Color.White,
                Height = 512,
                Width = 512
            };
            RightPictureWrapper = new PictureBoxWrapper(rightPictureBox);
            RightPictureWrapper.Click += new EventHandler(LoadImage);
            RightPictureWrapper.ContextMenuStrip = pictureBoxContextMenu;
            RightPictureWrapper.RegisterObserver(model);

            //Main Menu
            var menu = new MenuStrip();
            var fileItem = new ToolStripMenuItem()
            {
                Name = "File",
                Text = "File"
            };
            var saveItem = new ToolStripMenuItem()
            {
                Name = "Save",
                Text = "Save"
            };
            fileItem.DropDownItems.Add(saveItem);
            menu.Items.Add(fileItem);
            Controls.Add(menu);

            //Labels
            PsnrLabel = new Label() { Text = "dfwf" };

            //Buttons
            var swapButton = new Button() { Text = "Swap" };
            swapButton.Click += (sender,args)=> SwapImages();
            var psnrButton = new Button() { Text = "PSNR" };
            psnrButton.Click += (sender, args) => DisplayPSNR();
            var toYButton = new Button() { Text = "To Y" };
            toYButton.Click += (sender, args) => ToY();

            //Table
            var table = new TableLayoutPanel();
            var rowsCount = 5;
            var columnsCount = 2;
            for(var i = 0; i < rowsCount; i++)
            {
                table.RowStyles.Add(new RowStyle());
            }
            for (var i = 0; i < columnsCount; i++)
            {
                table.ColumnStyles.Add(new ColumnStyle());
            }
            table.Controls.Add(LeftPictureWrapper, 0, 0);
            table.Controls.Add(RightPictureWrapper, 1, 0);
            table.Controls.Add(swapButton, 0, 1);
            table.Controls.Add(psnrButton, 0, 2);
            table.Controls.Add(toYButton, 0, 3);
            table.Controls.Add(PsnrLabel, 0, 4);
            table.Dock = DockStyle.Fill;
            Controls.Add(table);
        }
        private void ToGrayScaleAvg(PictureBoxWrapper pictureBoxWrapper)
        {
            try
            {
                var bmp = new Bitmap(pictureBoxWrapper.Image);
                bmp.ToGrayscale(ColorInfo.AvgComponent);
                pictureBoxWrapper.Image = bmp;
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Picturebox is empty");
            }
        }
        private void ToGrayScaleLum(PictureBoxWrapper pictureBox)
        {
            try
            {
                var bmp = new Bitmap(pictureBox.Image);
                bmp.ToGrayscale(ColorInfo.Luminance);
                pictureBox.Image = bmp;
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Picturebox is empty");
            }
        }
        private void LoadImage(object sender, EventArgs e)
        {
            var pictureBoxWrapper = sender as PictureBoxWrapper;
            if (pictureBoxWrapper == null)
            {
                return;
            }
            var openFileDialog = new OpenFileDialog()
            {
                Title = "Open Image",
                InitialDirectory = @"C:\Test_Images",
                Filter = "All Files (*.*)|*.*|(*.png)|*.png|(*.jpg)|*.jpg|(*.jpeg)|*.jpeg|(*.gif)|*.gif|(*.bmp)|*.bmp",
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    var img = Image.FromFile(openFileDialog.FileName);
                    if (img.Height != 512 || img.Width != 512)
                    {
                        MessageBox.Show("Only 512x512 images are supported");
                        return;
                    }
                    pictureBoxWrapper.Image = img;
                }
                catch (System.IO.FileNotFoundException)
                {
                    MessageBox.Show("The specified file does not exist.");
                }
                catch (OutOfMemoryException)
                {
                    MessageBox.Show("The file does not have a valid image format.");
                }
            }
        }
        public void SwapImages()
        {
            try
            {
                Bitmap tmp = new Bitmap(LeftPictureWrapper.Image);
                LeftPictureWrapper.Image = RightPictureWrapper.Image;
                RightPictureWrapper.Image = tmp;
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Picturebox is empty");
            }
        }
        private void ContextMenuItem_Click(object sender, EventArgs e)
        {
            var toolStripMenuItem = sender as ToolStripMenuItem;
            if (toolStripMenuItem == null)
                return;
            var contextMenuStrip = toolStripMenuItem.Owner as ContextMenuStrip;
            if (contextMenuStrip == null)
                return;
            var pictureBoxWrapper = contextMenuStrip.SourceControl as PictureBoxWrapper;
            if (pictureBoxWrapper == null)
                return;
            switch (toolStripMenuItem.Text)
            {
                case "Duplicate":
                    DuplicateContent(pictureBoxWrapper);
                    break;
                case "Save":
                    SaveContent(pictureBoxWrapper);
                    break;
                case "Avg":
                    ToGrayScaleAvg(pictureBoxWrapper);
                    break;
                case "Lum":
                    ToGrayScaleLum(pictureBoxWrapper);
                    break;

            }
        }
        private void SaveContent(PictureBoxWrapper pictureBoxWrapper)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Images|*.bmp;*.png;*.jpg";
            ImageFormat format = ImageFormat.Png;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string ext = System.IO.Path.GetExtension(sfd.FileName);
                switch (ext)
                {
                    case ".bmp":
                        format = ImageFormat.Bmp;
                        break;
                    case ".jpg":
                        format = ImageFormat.Jpeg;
                        break;
                    case ".png":
                        format = ImageFormat.Png;
                        break;
                }
                try
                { 
                    pictureBoxWrapper.Image.Save(sfd.FileName, format);
                }
                catch (Exception ex) when (ex is ArgumentNullException || ex is NullReferenceException)
                {
                    MessageBox.Show("No image to save");
                }
            }
        }
        private void DuplicateContent(PictureBoxWrapper pictureBoxWrapper)
        {
            if (LeftPictureWrapper == pictureBoxWrapper)
            {
                RightPictureWrapper.Image = LeftPictureWrapper.Image;
            }
            LeftPictureWrapper.Image = RightPictureWrapper.Image;
        }
        private void DisplayPSNR()
        {
            MessageBox.Show(Model.CalculatePSNR(LeftPictureWrapper.Image, RightPictureWrapper.Image).ToString());
        }
        private void ToY()
        {
            RightPictureWrapper.Image = RgbToYcbcrConverter.ToYcbcrToRgb(new Bitmap(LeftPictureWrapper.Image));
        }
    }
}
