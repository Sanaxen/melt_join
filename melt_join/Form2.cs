using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace melt_join
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        string image_file = "";

        public void SetFile( string file)
        {
            image_file = file;
        }

        public System.Drawing.Image CreateImage(string filename)
        {
            System.Drawing.Image img = null;
            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(
                    filename,
                    System.IO.FileMode.Open,
                    System.IO.FileAccess.Read);
                img = System.Drawing.Image.FromStream(fs);
                fs.Close();
            }
            catch
            {
                img = null;
            }
            return img;
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.SizeMode == PictureBoxSizeMode.Zoom)
            {
                panel2.AutoScroll = true;
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                pictureBox1.Dock = DockStyle.None;
                pictureBox1.Refresh();
                return;
            }
            if (pictureBox1.SizeMode == PictureBoxSizeMode.AutoSize)
            {
                panel2.AutoScroll = true;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.Image = CreateImage(image_file);
                pictureBox1.Dock = DockStyle.Fill;
                pictureBox1.Refresh();
                return;
            }
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            //if (pictureBox1.SizeMode == PictureBoxSizeMode.Zoom)
            //{
            //    pictureBox1.Size = new System.Drawing.Size(new Point(panel2.Width, panel2.Height));
            //}
        }
    }
}
