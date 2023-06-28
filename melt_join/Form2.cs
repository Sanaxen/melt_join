using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace melt_join
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            InitializeAsync();
            webView21.NavigationCompleted += webView21_NavigationCompleted;
        }

        string image_file = "";
        string work_dir = "";
        string html = "";

        async void InitializeAsync()
        {
            try
            {
                await webView21.EnsureCoreWebView2Async(null);
            }
            catch (Exception)
            {
                MessageBox.Show("The WebView2 runtime may not be installed.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }

        private void webView21_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (webView21.CoreWebView2 != null)
            {
                //Web画面からVB/C＃へのホストオブジェクトにアクセスする必要がなければ
                webView21.CoreWebView2.Settings.AreHostObjectsAllowed = false;

                //Webコンテンツ(JavaScript)からVB／C＃側へのメッセージを処理する必要がなければ
                //webView21.CoreWebView2.Settings.IsWebMessageEnabled = false;

                //Web画面でJavaScriptを使用したくなければ
                //webView21.CoreWebView2.Settings.IsScriptEnabled = false;

                //alertやpromptなどのダイアログを表示したくなければ
                webView21.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
            }
        }

        bool loadHtml()
        {
            if (System.IO.File.Exists(html))
            {
                string webpath = html.Replace("\\", "/").Replace("//", "/");
                webpath = "file:///" + webpath;
                try
                {
                    webView21.Source = new Uri(webpath);
                    if (webView21.CoreWebView2 != null)
                    {
                        //webView21.CoreWebView2.Navigate(webpath);
                    }
                    //webView21.Reload();
                    webView21.Update();
                    webView21.Refresh();
                }
                catch {
                    return false;
                }
            }else
            {
                return false;
            }

            return true;
        }

        public void SetFile( string dir, string file)
        {
            image_file = file;
            work_dir = dir;
            html = Path.ChangeExtension(dir + "\\"+file, null)+".html";
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
            if ( loadHtml() && checkBox1.Checked)
            {
                pictureBox1.Hide();
                webView21.Show();
            }else
            {
                pictureBox1.Show();
                webView21.Hide();
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
