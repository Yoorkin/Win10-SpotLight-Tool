using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GetBg
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoA")]
        static extern Int32 SystemParametersInfo(Int32 uAction, Int32 uParam, string lpvParam, Int32 fuWinIni);
        private const int SPI_SETDESKWALLPAPER = 20;
        public Form1()
        {
            InitializeComponent();
        }
        static string basePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\锁屏壁纸";
        static string phone = basePath + "\\phone";
        static string desktop = basePath + "\\desktop";
        static FileInfo[] imgView = new DirectoryInfo(desktop).GetFiles();
        static int imgIndex = imgView.Length-1;
        private void Form1_Load(object sender, EventArgs e)
        {
            if (Directory.Exists(basePath + "EN"))
            {
                Directory.CreateDirectory(basePath + "EN");
                setBtn.Text = "Set as wallpaper";
                openBtn.Text = "Reveal in explorer";
                cleanBtn.Text = "Clear all pictures";
                linkLabel2.Text = "中文";
            }
            linkLabel1.Links.Add(3, 9);
            getImages();
            picBox.Image = Image.FromFile(imgView[imgView.Length-1].FullName);
        }
        public static void getImages()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Packages";
            List<string> dirs = new List<string>(Directory.EnumerateDirectories(path));
            string content = "Microsoft.Windows.ContentDeliveryManager";
            string target = "";
            foreach (var file in new DirectoryInfo(path).GetDirectories())
            {
                if (file.Name.Length > content.Length && file.Name.Substring(0, content.Length) == content)
                {
                    target = file.FullName; ;
                    break;
                }
            }
            Directory.CreateDirectory(basePath);
            Directory.CreateDirectory(desktop);
            Directory.CreateDirectory(phone);
            int count = 0;
            foreach (var file in new DirectoryInfo(target + "\\LocalState\\Assets").GetFiles())
            {
                System.IO.File.Copy(file.FullName, basePath + "\\" + file.Name + ".jpg");
            }
            foreach (var imgFile in new DirectoryInfo(basePath).GetFiles())
            {

                Image pic = Image.FromFile(imgFile.FullName);
                if (pic.Width < 1000)
                {
                    pic.Dispose();
                    imgFile.Delete();
                    continue;
                }
                if (pic.Width > pic.Height)
                {
                    pic.Dispose();
                    pic.Dispose();
                    string trg = desktop + "\\" + imgFile.Name;
                    if (File.Exists(trg))
                        imgFile.Delete();
                    else
                        File.Move(imgFile.FullName, trg);
                }
                else
                {
                    pic.Dispose();
                    string trg = phone + "\\" + imgFile.Name;
                    if (File.Exists(trg))
                        imgFile.Delete();
                    else
                        File.Move(imgFile.FullName, trg);
                }
            }
        }

        private void setBtn_Click(object sender, EventArgs e)
        {
            string tmp = basePath + "\\使用中的壁纸.jpg";
            File.Delete(tmp);
            File.Copy(imgView[imgIndex%(imgView.Length-1)].FullName, tmp);
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, tmp , 1); ;
        }

        private void cleanBtn_Click(object sender, EventArgs e)
        {
            DialogResult result;
            if (Directory.Exists(basePath + "EN"))
                result = MessageBox.Show("Are you sure to clear all pictures?", "Attention", MessageBoxButtons.OKCancel);
            else
                result = MessageBox.Show("确定要清空Windows聚焦壁纸吗？\n（包括以前保存的壁纸)", "提示", MessageBoxButtons.OKCancel);
            if(result == DialogResult.OK)
            {
                foreach (var file in new DirectoryInfo(phone).GetFiles()) file.Delete();
                foreach (var file in new DirectoryInfo(desktop).GetFiles()) file.Delete();
            }
        }

        private void openBtn_Click(object sender, EventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "explorer.exe";
            p.StartInfo.Arguments = basePath;
            p.Start();
        }

        private void preBtn_Click(object sender, EventArgs e)
        {
            picBox.Image.Dispose();
            imgIndex--;
            if (imgIndex == 0) imgIndex = imgView.Length - 1;
            picBox.Image = Image.FromFile(imgView[imgIndex % (imgView.Length - 1)].FullName);
            picBox.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void nextBtn_Click(object sender, EventArgs e)
        {
            picBox.Image.Dispose();
            imgIndex++;
            picBox.Image = Image.FromFile(imgView[imgIndex % (imgView.Length - 1)].FullName);
            picBox.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void picBox_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", @"https://github.com/Yoorkin/Win10-SpotLight-Tool");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(Directory.Exists(basePath + "EN"))
            {
                Directory.Delete(basePath + "EN");
                setBtn.Text = "设为壁纸";
                openBtn.Text = "打开壁纸文件夹";
                cleanBtn.Text = "清空壁纸";
                linkLabel2.Text = "English";
            }
            else
            {
                Directory.CreateDirectory(basePath + "EN");
                setBtn.Text = "Set as wallpaper";
                openBtn.Text = "Reveal in explorer";
                cleanBtn.Text = "Clear all pictures";
                linkLabel2.Text = "中文";
            }
        }
    }
}
