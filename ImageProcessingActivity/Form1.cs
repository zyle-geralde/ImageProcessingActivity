using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebCamLib;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Diagnostics.Tracing;
using System.Collections.Concurrent;
using static System.Net.Mime.MediaTypeNames;

namespace ImageProcessingActivity
{
    public partial class Form1 : Form
    {
        Bitmap loaded, processed,imagea,imageb, colorgreen, resultImage;
        FilterInfoCollection fic;
        VideoCaptureDevice vcd;
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void basicCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    processed.SetPixel(x, y, pixel);
                }
            }
            pictureBox2.Image = processed;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            processed.Save(saveFileDialog1.FileName);
        }

        private void greyScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            int ave;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    ave = (int)(pixel.R + pixel.G + pixel.B) / 3;
                    Color gray = Color.FromArgb(ave, ave, ave);
                    processed.SetPixel(x, y, gray);
                }
            }
            pictureBox2.Image = processed;
        }

        private void colorInversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    Color inver = Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B);

                    processed.SetPixel(x, y, inver);
                }
            }
            pictureBox2.Image = processed;
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;

            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);

                    int originalRed = pixel.R;
                    int originalGreen = pixel.G;
                    int originalBlue = pixel.B;

                    int sepiaRed = (int)(0.393 * originalRed + 0.769 * originalGreen + 0.189 * originalBlue);
                    int sepiaGreen = (int)(0.349 * originalRed + 0.686 * originalGreen + 0.168 * originalBlue);
                    int sepiaBlue = (int)(0.272 * originalRed + 0.534 * originalGreen + 0.131 * originalBlue);

                    sepiaRed = Math.Min(255, sepiaRed);
                    sepiaGreen = Math.Min(255, sepiaGreen);
                    sepiaBlue = Math.Min(255, sepiaBlue);

                    Color sepiaColor = Color.FromArgb(sepiaRed, sepiaGreen, sepiaBlue);
                    processed.SetPixel(x, y, sepiaColor);
                }
            }

            pictureBox2.Image = processed;
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DIPClass.Histogram(ref loaded, ref processed);
            pictureBox2.Image = processed;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //deviceList = DeviceManager.GetAllDevices();
        }

        private void onToolStripMenuItem_Click(object sender, EventArgs e)
        {

            fic = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            foreach(FilterInfo dev in fic)
            {
                comboBox1.Items.Add(dev.Name);
                comboBox1.SelectedIndex = 0;
                vcd = new VideoCaptureDevice();
                vcd = new VideoCaptureDevice(fic[comboBox1.SelectedIndex].MonikerString);
                vcd.NewFrame += FinalFrame_NewFrame;
                vcd.Start();

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
            
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            imageb = new Bitmap(openFileDialog2.FileName);
            pictureBox3.Image = imageb;
        }

        private void openFileDialog3_FileOk(object sender, CancelEventArgs e)
        {
            imagea = new Bitmap(openFileDialog3.FileName);
            pictureBox4.Image = imagea;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog3.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Set mygreen to pure green
            Color mygreen = Color.FromArgb(0, 255, 0);
            int greygreen = (mygreen.R + mygreen.G + mygreen.B) / 3;
            int threshold = 5;
            resultImage = new Bitmap(imageb.Width, imageb.Height);

            for (int x = 0; x < imageb.Width; x++)
            {
                for (int y = 0; y < imageb.Height; y++)
                {
                    Color pixel = imageb.GetPixel(x, y);
                    Color backpixel = imagea.GetPixel(x, y);
                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
                    int subtractvalue = Math.Abs(grey - greygreen);

                    if (subtractvalue <= threshold)
                        resultImage.SetPixel(x, y, backpixel);
                    else
                        resultImage.SetPixel(x, y, pixel);
                }
            }

            pictureBox5.Image = resultImage;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            saveFileDialog2.ShowDialog();
        }

        private void saveFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            resultImage.Save(saveFileDialog2.FileName);
        }

        private void FinalFrame_NewFrame(object sender, NewFrameEventArgs e)
        {
            // Clone the frame and enqueue it for processing
            Bitmap bmap = (Bitmap)e.Frame.Clone();
            pictureBox1.Image = bmap;
        }

        private void offToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (vcd != null && vcd.IsRunning)
            {
                vcd.SignalToStop();  // Stop the capture
                vcd.WaitForStop();   // Wait for the capture to stop
                vcd = null;          // Release the video capture device
                pictureBox1.Image = null;
                pictureBox2.Image = null;
            }
        }

        private void greyScaleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            /*IDataObject data;
            Image bmap;
            deviceList[0].Sendmessage();
            data = Clipboard.GetDataObject();
            bmap = (Image)(data.GetData("System.Drawing.Bitmap", true));
            Bitmap b = new Bitmap(bmap);
            processed = new Bitmap(b.Width, b.Height);
            Color pixel;
            int ave;
            for (int x = 0; x < b.Width; x++)
            {
                for (int y = 0; y < b.Height; y++)
                {
                    pixel = b.GetPixel(x, y);
                    ave = (int)(pixel.R + pixel.G + pixel.B) / 3;
                    Color gray = Color.FromArgb(ave, ave, ave);
                    processed.SetPixel(x, y, gray);
                }
            }
            pictureBox2.Image = processed;*/
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            loaded = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = loaded;
        }

    }
}
