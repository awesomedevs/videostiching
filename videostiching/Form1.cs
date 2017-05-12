using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Stitching;
using Emgu.CV.Util;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace videostiching
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            GetFrameFromVideos();
        }

        Mat result = new Mat();
        Mat[] sourceImages = new Mat[2];

        //Faz stich de dois frames de dois video separados
        private void stichFirstFrameToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
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
                            //por algum motivo a imagem fica rodada :(
                            bt.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                            pictureBox1.Image = bt;
                            // pictureBox1.Image.Save(@"path", ImageFormat.Jpeg);
                        }
                        else
                        {
                            MessageBox.Show(this, String.Format("Stiching Error: {0}", stitchStatus));
                            pictureBox1.Image = null;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        /*Technically, warpPerspective do this on the coordinates of the Mat and move the pixel value(color) to a new pixel. perspectiveTransform, it just compute the new coordinate of the point and store it in the new vector.*/
        //O ideal que seria escolher com clicks os cantos e cento do campo, para executar a perspetiva, não sei se será melhor abordagem
        private void wrapPrespectiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PointF[] srcs = new PointF[4];
            srcs[0] = new PointF(0, 0);
            srcs[1] = new PointF(pictureBox1.Height, 0);
            srcs[2] = new PointF(0, pictureBox1.Width);
            srcs[3] = new PointF(pictureBox1.Height, pictureBox1.Width);

            PointF[] dscs = new PointF[4];
            dscs[0] = new PointF(0, 0);
            dscs[1] = new PointF(pictureBox1.Height, 0);
            dscs[2] = new PointF(0, pictureBox1.Width);
            dscs[3] = new PointF(pictureBox1.Height, pictureBox1.Width);

            //Info acerca do Wrap Prespective (Não sei se é a melhor soluçao para isto)
            //https://goo.gl/eBt9fn
            using (var matrix = CvInvoke.GetPerspectiveTransform(srcs, dscs))
            {
                using (var cutImagePortion = new Mat())
                {
                    //Cuidado tambem com o size, em baixo
                    CvInvoke.WarpPerspective(sourceImages[0], cutImagePortion, matrix, new Size(pictureBox1.Height, pictureBox1.Width), Inter.Cubic);
                    Bitmap bt = new Bitmap(cutImagePortion.Bitmap);
                    //por algum motivo a imagem fica rodada :(
                    bt.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                    pictureBox1.Image = bt;
                }
            }
        }

        //private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        //{
        //    Point controlRelative = pictureBox1.PointToClient(MousePosition);
        //    Size imageSize = pictureBox1.Image.Size;
        //    Size boxSize = pictureBox1.Size;
        //    Point imagePosition = new Point((imageSize.Width / boxSize.Width) * controlRelative.X, (imageSize.Height / boxSize.Height) * controlRelative.Y);
        //    myPoints.Add(new PointF(imagePosition.X, imagePosition.Y));

        //    MessageBox.Show(string.Format("X: {0} Y: {1}", imagePosition.X, imagePosition.Y));
        //}

        public void GetFrameFromVideos()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.Multiselect = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Capture capt1 = new Capture(dlg.FileNames[0]?.ToString());
                Capture capt2 = new Capture(dlg.FileNames[1]?.ToString());
                sourceImages[0] = capt1.QueryFrame();
                sourceImages[1] = capt2.QueryFrame();
            }
        }
    }
}
