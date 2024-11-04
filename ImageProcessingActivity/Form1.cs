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
using System.Diagnostics;

namespace ImageProcessingActivity
{
    public partial class Form1 : Form
    {
        Bitmap loaded, processed,imagea,imageb, colorgreen, resultImage;
        private int isButtonClicked = 0;
        FilterInfoCollection fic;
        VideoCaptureDevice vcd;
        public Form1()
        {
            InitializeComponent();
            this.FormClosing += Form1_FormClosing;
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
                isButtonClicked = 0;
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
            resultImage = new Bitmap(imagea.Width, imagea.Height);
            Color mygreen = Color.FromArgb(0, 0, 255);
            int greygreen = (mygreen.R + mygreen.G + mygreen.B) / 3;
            int threshold = 5;

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

        private void basicCopyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 2;
        }

        private void colorInversionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 3;
        }

        private void histogramToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 4;
        }

        private void sepiaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 5;
        }

        private void FinalFrame_NewFrame(object sender, NewFrameEventArgs e)
        {
            
            Bitmap originalFrame = (Bitmap)e.Frame.Clone();
            pictureBox1.Image = originalFrame;

            if (isButtonClicked == -1) 
            {
               
                pictureBox1.Image = null;
                pictureBox2.Image = null;

                
                if (vcd != null && vcd.IsRunning)
                {
                    vcd.SignalToStop();  
                    vcd.WaitForStop();   
                    vcd = null;          
                }
                isButtonClicked = 0;
                return;  
            }
            else if(isButtonClicked == 1)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                Bitmap grayscaleFrame = new Bitmap(bmap.Width, bmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                var rect = new Rectangle(0, 0, bmap.Width, bmap.Height);
                var bmapData = bmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bmap.PixelFormat);
                var grayscaleData = grayscaleFrame.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, grayscaleFrame.PixelFormat);

                int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(bmap.PixelFormat) / 8;
                int stride = bmapData.Stride;
                IntPtr scan0 = bmapData.Scan0;
                IntPtr grayScan0 = grayscaleData.Scan0;

                unsafe
                {
                    byte* p = (byte*)(void*)scan0;
                    byte* pGray = (byte*)(void*)grayScan0;

                    for (int y = 0; y < bmap.Height; y++)
                    {
                        for (int x = 0; x < bmap.Width; x++)
                        {
                            int index = y * stride + x * bytesPerPixel;

                            byte grayValue = (byte)((p[index + 2] + p[index + 1] + p[index]) / 3);

                            pGray[index] = grayValue;
                            pGray[index + 1] = grayValue;
                            pGray[index + 2] = grayValue;
                        }
                    }
                }

                bmap.UnlockBits(bmapData);
                grayscaleFrame.UnlockBits(grayscaleData);

                
                pictureBox2.Image = grayscaleFrame;
            }
            else if(isButtonClicked == 2)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                Bitmap grayscaleFrame = new Bitmap(bmap.Width, bmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                var rect = new Rectangle(0, 0, bmap.Width, bmap.Height);
                var loadedData = bmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bmap.PixelFormat);
                var processedData = grayscaleFrame.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, grayscaleFrame.PixelFormat);

                int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(bmap.PixelFormat) / 8;
                int stride = loadedData.Stride;
                IntPtr scan0Loaded = loadedData.Scan0;
                IntPtr scan0Processed = processedData.Scan0;

                unsafe
                {
                    byte* pLoaded = (byte*)(void*)scan0Loaded;
                    byte* pProcessed = (byte*)(void*)scan0Processed;

                    for (int y = 0; y < bmap.Height; y++)
                    {
                        for (int x = 0; x < bmap.Width; x++)
                        {
                            int index = y * stride + x * bytesPerPixel;

                            
                            pProcessed[index] = pLoaded[index];           
                            pProcessed[index + 1] = pLoaded[index + 1];   
                            pProcessed[index + 2] = pLoaded[index + 2];   
                        }
                    }
                }

                bmap.UnlockBits(loadedData);
                grayscaleFrame.UnlockBits(processedData);

                
                pictureBox2.Image = grayscaleFrame;
            }
            else if(isButtonClicked == 3)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                Bitmap grayscaleFrame = new Bitmap(bmap.Width, bmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                var rect = new Rectangle(0, 0, bmap.Width, bmap.Height);
                var loadedData = bmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bmap.PixelFormat);
                var processedData = grayscaleFrame.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, grayscaleFrame.PixelFormat);

                int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(bmap.PixelFormat) / 8;
                int stride = loadedData.Stride;
                IntPtr scan0Loaded = loadedData.Scan0;
                IntPtr scan0Processed = processedData.Scan0;

                unsafe
                {
                    byte* pLoaded = (byte*)(void*)scan0Loaded;
                    byte* pProcessed = (byte*)(void*)scan0Processed;

                    for (int y = 0; y < bmap.Height; y++)
                    {
                        for (int x = 0; x < bmap.Width; x++)
                        {
                            int index = y * stride + x * bytesPerPixel;

                           
                            pProcessed[index] = (byte)(255 - pLoaded[index]);       
                            pProcessed[index + 1] = (byte)(255 - pLoaded[index + 1]); 
                            pProcessed[index + 2] = (byte)(255 - pLoaded[index + 2]); 
                        }
                    }
                }

                bmap.UnlockBits(loadedData);
                grayscaleFrame.UnlockBits(processedData);

                
                pictureBox2.Image = grayscaleFrame;

            }
            else if(isButtonClicked == 4)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                // Initialize histogram data
                int[] histogramData = new int[256];

                // Convert to grayscale and calculate histogram using LockBits
                var rect = new Rectangle(0, 0, bmap.Width, bmap.Height);
                var frameData = bmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bmap.PixelFormat);

                int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(bmap.PixelFormat) / 8;
                int stride = frameData.Stride;
                IntPtr scan0 = frameData.Scan0;

                unsafe
                {
                    byte* p = (byte*)(void*)scan0;

                    for (int y = 0; y < bmap.Height; y++)
                    {
                        for (int x = 0; x < bmap.Width; x++)
                        {
                            int index = y * stride + x * bytesPerPixel;

                            // Convert to grayscale
                            byte grayValue = (byte)((p[index] + p[index + 1] + p[index + 2]) / 3);

                            // Increment histogram count for the grayscale value
                            histogramData[grayValue]++;
                        }
                    }
                }

                bmap.UnlockBits(frameData);

                // Create histogram image
                Bitmap histogramImage = new Bitmap(256, 800);
                using (Graphics g = Graphics.FromImage(histogramImage))
                {
                    g.Clear(Color.White); // Set background to white
                }

                // Draw histogram on histogramImage
                int maxHeight = histogramImage.Height - 1;
                for (int i = 0; i < histogramData.Length; i++)
                {
                    int height = Math.Min(histogramData[i] / 5, maxHeight);  // Scale down for visualization
                    for (int j = 0; j < height; j++)
                    {
                        histogramImage.SetPixel(i, maxHeight - j, Color.Black);
                    }
                }

                // Display histogram in pictureBox2
                pictureBox2.Image = histogramImage;
            }
            else if(isButtonClicked == 5)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                // Create the processed bitmap
                Bitmap grayscaleFrame = new Bitmap(bmap.Width, bmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                // Define a rectangle for the entire image area
                var rect = new Rectangle(0, 0, bmap.Width, bmap.Height);

                // Lock bits for both input and output images
                var loadedData = bmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bmap.PixelFormat);
                var processedData = grayscaleFrame.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, grayscaleFrame.PixelFormat);

                int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(bmap.PixelFormat) / 8;
                int stride = loadedData.Stride;
                IntPtr scan0 = loadedData.Scan0;
                IntPtr processedScan0 = processedData.Scan0;

                unsafe
                {
                    byte* pLoaded = (byte*)(void*)scan0;
                    byte* pProcessed = (byte*)(void*)processedScan0;

                    for (int y = 0; y < bmap.Height; y++)
                    {
                        for (int x = 0; x < bmap.Width; x++)
                        {
                            int index = y * stride + x * bytesPerPixel;

                            // Get the RGB components
                            int originalRed = pLoaded[index + 2];
                            int originalGreen = pLoaded[index + 1];
                            int originalBlue = pLoaded[index];

                            // Apply sepia transformation
                            int sepiaRed = (int)(0.393 * originalRed + 0.769 * originalGreen + 0.189 * originalBlue);
                            int sepiaGreen = (int)(0.349 * originalRed + 0.686 * originalGreen + 0.168 * originalBlue);
                            int sepiaBlue = (int)(0.272 * originalRed + 0.534 * originalGreen + 0.131 * originalBlue);

                            // Ensure values stay within byte range (0–255)
                            pProcessed[index + 2] = (byte)Math.Min(255, sepiaRed);
                            pProcessed[index + 1] = (byte)Math.Min(255, sepiaGreen);
                            pProcessed[index] = (byte)Math.Min(255, sepiaBlue);
                        }
                    }
                }

                // Unlock bits for both images
                bmap.UnlockBits(loadedData);
                grayscaleFrame.UnlockBits(processedData);

                // Set the processed image to PictureBox
                pictureBox2.Image = grayscaleFrame;
            }
            else
            {
                pictureBox2.Image = null;
            }



        }

        private void offToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isButtonClicked = -1;

            /*if (vcd != null && vcd.IsRunning)
            {
                vcd.SignalToStop();  // Stop the capture
                vcd.WaitForStop();   // Wait for the capture to stop
                vcd = null;          // Release the video capture device
            }*/

        }

        private void greyScaleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 1;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            loaded = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = loaded;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            isButtonClicked = -1;
        }

    }
}
