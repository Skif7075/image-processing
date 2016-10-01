using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using image_processing.Extensions;

namespace image_processing
{
    class MainForm : Form
    {
        public PictureBox LeftPictureBox { get; }
        public PictureBox RightPictureBox { get; }
        private Model Model { get; }

        public MainForm(Model model)
        {
            Model = model;
            Width = 1100;
            Height = 700;
            LeftPictureBox = new PictureBox()
            {
                BackColor = Color.White,
                Height = 512,
                Width = 512
            };
            LeftPictureBox.Click += new EventHandler(LoadImage);
            var pictureBoxContextMenu = new ContextMenuStrip();
            var saveContextItem = new ToolStripMenuItem("Save");
            pictureBoxContextMenu.Items.Add(saveContextItem);
            saveContextItem.Click += new EventHandler(SaveContextItem_Click);
            LeftPictureBox.ContextMenuStrip = pictureBoxContextMenu;
            RightPictureBox = new PictureBox()
            {
                BackColor = Color.White,
                Height = 512,
                Width = 512
            };
            RightPictureBox.Click += new EventHandler(LoadImage);
            RightPictureBox.ContextMenuStrip = pictureBoxContextMenu;
            var avgButton = new Button() { Text = "Avg" };
            avgButton.Click += new EventHandler(AvgButton_Click);
            var lumButton = new Button() { Text = "Luminance" }; 
            lumButton.Click += new EventHandler(LumButton_Click);
            var swapButton = new Button() { Text = "Swap" };
            swapButton.Click += (sender,args)=> SwapImages();

            var table = new TableLayoutPanel();
            var rowsCount = 4;
            var columnsCount = 2;
            for(var i = 0; i < rowsCount; i++)
            {
                table.RowStyles.Add(new RowStyle());
            }
            for (var i = 0; i < columnsCount; i++)
            {
                table.ColumnStyles.Add(new ColumnStyle());
            }
            table.Controls.Add(LeftPictureBox, 0, 0);
            table.Controls.Add(RightPictureBox, 1, 0);
            table.Controls.Add(avgButton, 0, 1);
            table.Controls.Add(lumButton, 0, 2);
            table.Controls.Add(swapButton, 0, 3);
            table.Dock = DockStyle.Fill;
            Controls.Add(table);
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
        }
        private void AvgButton_Click(object sender, EventArgs e)
        {
            try
            {
                RightPictureBox.Image = (new Bitmap(LeftPictureBox.Image)).ToGrayscale(ColorInfo.AvgComponent);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Picturebox is empty");
            }
        }
        private void LumButton_Click(object sender, EventArgs e)
        {
            try
            {
                RightPictureBox.Image = (new Bitmap(LeftPictureBox.Image)).ToGrayscale(ColorInfo.Luminance);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Picturebox is empty");
            }
        }
        private void LoadImage(object sender, EventArgs e)
        {
            var pictureBox = sender as PictureBox;
            if (pictureBox == null)
            {
                return;
            }
            var openFileDialog = new OpenFileDialog()
            {
                Title = "Open Image",
                InitialDirectory = @"C:\Test_Images",
                Filter = "(*.png)|*.png|(*.jpg)|*.jpg|(*.jpeg)|*.jpeg|(*.gif)|*.gif|(*.bmp)|*.bmp",
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    pictureBox.Image = Image.FromFile(openFileDialog.FileName);
                }
                catch (System.IO.FileNotFoundException)
                {
                    MessageBox.Show("Failed to load image");
                }
            }
        }
        public void SwapImages()
        {
            try
            {
                Bitmap tmp = new Bitmap(LeftPictureBox.Image);
                LeftPictureBox.Image = RightPictureBox.Image;
                RightPictureBox.Image = tmp;
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Picturebox is empty");
            }
        }
        private void SaveContextItem_Click(object sender, EventArgs e)
        {
            var toolStripMenuItem = sender as ToolStripMenuItem;
            if (toolStripMenuItem == null)
                return;
            var contextMenuStrip = toolStripMenuItem.Owner as ContextMenuStrip;
            if (contextMenuStrip == null)
                return;
            var pictureBox = contextMenuStrip.SourceControl as PictureBox;
            if (pictureBox == null)
                return;
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
                    pictureBox.Image.Save(sfd.FileName, format);
                }
                catch (Exception ex) when (ex is ArgumentNullException || ex is NullReferenceException)
                {
                    MessageBox.Show("No image to save");
                }
            }
        }
    }
}
