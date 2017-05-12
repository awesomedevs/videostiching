using Emgu.CV;
using Emgu.CV.Stitching;
using Emgu.CV.Util;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace videostiching
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void stichFirstFrameToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.Multiselect = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Mat[] sourceImages = new Mat[2];
                Mat result = new Mat();

                Capture capt1 = new Capture(dlg.FileNames[0].ToString());
                Capture capt2 = new Capture(dlg.FileNames[1].ToString());
                sourceImages[0] = capt1.QueryFrame();
                sourceImages[1] = capt2.QueryFrame();
                try
                {
                    using (Stitcher stitcher = new Stitcher(true))
                    {
                        using (VectorOfMat vm = new VectorOfMat())
                        {
                            vm.Push(sourceImages);

                            var stitchStatus = stitcher.Stitch(vm, result);

                            if (stitchStatus)
                            {
                                Bitmap bt = new Bitmap(result.Bitmap);
                                bt.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                                pictureBox1.Image = bt;
                               // pictureBox1.Image.Save(@"C:\Users\lanun-surf\Desktop\stich.jpeg", ImageFormat.Jpeg);
                            }
                            else
                            {
                                MessageBox.Show(this, String.Format("Stiching Error: {0}", stitchStatus));
                                pictureBox1.Image = null;
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                }
            }
        }
    }
}
