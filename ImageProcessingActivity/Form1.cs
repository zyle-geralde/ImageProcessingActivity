﻿using System;
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
using ImageProcess2;

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
            if(pictureBox1.Image == null)
            {
                return;
            }
            if(loaded == null)
            {
                return;
            }
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
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
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
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
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
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
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
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
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

        private void horizontalFlipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(loaded.Width - 1 - x, y);

                    processed.SetPixel(x, y, pixel);
                }
            }
            pictureBox2.Image = processed;
        }

        private void verticalFlipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, loaded.Height - 1 - y);

                    processed.SetPixel(x, y, pixel);
                }
            }
            pictureBox2.Image = processed;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if(pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            processed = new Bitmap(loaded.Width, loaded.Height);
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    Color temp = loaded.GetPixel(x, y);
                    Color changed;
                    if (trackBar1.Value > 0)
                        changed = Color.FromArgb(Math.Min(temp.R + trackBar1.Value, 255), Math.Min(temp.G + trackBar1.Value, 255), Math.Min(temp.B + trackBar1.Value, 255));
                    else
                        changed = Color.FromArgb(Math.Max(temp.R + trackBar1.Value, 0), Math.Max(temp.G + trackBar1.Value, 0), Math.Max(temp.B + trackBar1.Value, 0));

                    processed.SetPixel(x, y, changed);
                }
            }
            pictureBox2.Image = processed;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if(pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            DIPClass.Equalisation(loaded, ref processed, trackBar2.Value);
            pictureBox2.Image = processed;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            DIPClass.Rotate(loaded, ref processed, trackBar3.Value);
            pictureBox2.Image = processed;
        }

        private void scaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            DIPClass.Scale(ref loaded, ref processed, 100, 100);
            pictureBox2.Image = processed;
        }

        private void binaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            DIPClass.BinaryThreshold(ref loaded, ref processed, 180);
            pictureBox2.Image = processed;

        }

        private void verticalFlipToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 6;
        }

        private void horizontalFlipToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 7;
        }

        private void scaleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 8;
        }

        private void binaryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 9;
        }

        private void smoothToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            DIPClass.Smooth(loaded, ref processed,1);
            pictureBox2.Image = processed;
        }

        private void gaussianBlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            DIPClass.GaussianBlur(loaded, ref processed,4);
            pictureBox2.Image = processed;
        }

        private void sharpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            DIPClass.Sharpen(loaded, ref processed, 11);
            pictureBox2.Image = processed;
        }

        private void meanRemovalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            DIPClass.MeanRemoval(loaded, ref processed, 9);
            pictureBox2.Image = processed;
        }

        private void embossLaplacianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            DIPClass.Emboss_Laplacian(loaded, ref processed);
            pictureBox2.Image = processed;
        }

        private void embossHorzVertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            DIPClass.EmbossHorzVert(loaded, ref processed);
            pictureBox2.Image = processed;
        }

        private void embossAllDirectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            DIPClass.Emboss_AllDirection(loaded, ref processed);
            pictureBox2.Image = processed;
        }

        private void embossLossyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            DIPClass.EmbossLossy(loaded, ref processed);
            pictureBox2.Image = processed;
        }

        private void embossHorizontalOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            DIPClass.EmbossHorizontalOnly(loaded, ref processed);
            pictureBox2.Image = processed;
        }

        private void embossVerticalOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            DIPClass.EmbossVerticalOnly(loaded, ref processed);
            pictureBox2.Image = processed;
        }

        private void smoothToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 11;
        }

        private void gaussianBlurToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 12;
        }

        private void sharpenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 13;
        }

        private void meanRemovalToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 14;
        }

        private void embossLaplacianToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 15;
        }

        private void embossHorzVertToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 16;
        }

        private void embossAllDirectionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 17;
        }

        private void embossLossyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 18;
        }

        private void embossHorizontalOnlyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 19;
        }

        private void embossVerticalOnlyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 20;
        }

        private void edgeDetectQuickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            Bitmap m = (Bitmap)pictureBox1.Image.Clone();
            BitmapFilter.EdgeDetectQuick(m);
            pictureBox2.Image = m;
        }

        private void prewittToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            Bitmap m = (Bitmap)pictureBox1.Image.Clone();
            BitmapFilter.EdgeDetectConvolution(m,2,75);
            pictureBox2.Image = m;
        }

        private void sobelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            Bitmap m = (Bitmap)pictureBox1.Image.Clone();
            BitmapFilter.EdgeDetectConvolution(m, 3, 75);
            pictureBox2.Image = m;
        }

        private void kirshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            Bitmap m = (Bitmap)pictureBox1.Image.Clone();
            BitmapFilter.EdgeDetectConvolution(m, 1, 75);
            pictureBox2.Image = m;
        }

        private void horizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            Bitmap m = (Bitmap)pictureBox1.Image.Clone();
            BitmapFilter.EdgeDetectHorizontal(m);
            pictureBox2.Image = m;
        }

        private void verticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            Bitmap m = (Bitmap)pictureBox1.Image.Clone();
            BitmapFilter.EdgeDetectVertical(m);
            pictureBox2.Image = m;
        }

        private void homogenityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            Bitmap m = (Bitmap)pictureBox1.Image.Clone();
            BitmapFilter.EdgeDetectHomogenity(m,75);
            pictureBox2.Image = m;
        }

        private void differenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            Bitmap m = (Bitmap)pictureBox1.Image.Clone();
            BitmapFilter.EdgeDetectDifference(m,75);
            pictureBox2.Image = m;
        }

        private void edgeEnhanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            if (loaded == null)
            {
                return;
            }
            Bitmap m = (Bitmap)pictureBox1.Image.Clone();
            BitmapFilter.EdgeEnhance(m, 75);
            pictureBox2.Image = m;
        }

        private void quickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isButtonClicked = 21;
        }

        private void sobelToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 22;
        }

        private void prewittToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 23;
        }

        private void kirshToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 24;
        }

        private void horizontalToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 25;
        }

        private void verticalToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 26;
        }

        private void homogentiyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isButtonClicked = 27;
        }

        private void differenceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 28;
        }

        private void edgeEnhanceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isButtonClicked = 29;
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
                
                int[] histogramData = new int[256];

                
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

                            
                            byte grayValue = (byte)((p[index] + p[index + 1] + p[index + 2]) / 3);

                            
                            histogramData[grayValue]++;
                        }
                    }
                }

                bmap.UnlockBits(frameData);

                
                Bitmap histogramImage = new Bitmap(256, 800);
                using (Graphics g = Graphics.FromImage(histogramImage))
                {
                    g.Clear(Color.White); 
                }

                
                int maxHeight = histogramImage.Height - 1;
                for (int i = 0; i < histogramData.Length; i++)
                {
                    int height = Math.Min(histogramData[i] / 5, maxHeight);  
                    for (int j = 0; j < height; j++)
                    {
                        histogramImage.SetPixel(i, maxHeight - j, Color.Black);
                    }
                }

               
                pictureBox2.Image = histogramImage;
            }
            else if(isButtonClicked == 5)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                
                Bitmap grayscaleFrame = new Bitmap(bmap.Width, bmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                
                var rect = new Rectangle(0, 0, bmap.Width, bmap.Height);

                
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

                            
                            int originalRed = pLoaded[index + 2];
                            int originalGreen = pLoaded[index + 1];
                            int originalBlue = pLoaded[index];

                            
                            int sepiaRed = (int)(0.393 * originalRed + 0.769 * originalGreen + 0.189 * originalBlue);
                            int sepiaGreen = (int)(0.349 * originalRed + 0.686 * originalGreen + 0.168 * originalBlue);
                            int sepiaBlue = (int)(0.272 * originalRed + 0.534 * originalGreen + 0.131 * originalBlue);

                            
                            pProcessed[index + 2] = (byte)Math.Min(255, sepiaRed);
                            pProcessed[index + 1] = (byte)Math.Min(255, sepiaGreen);
                            pProcessed[index] = (byte)Math.Min(255, sepiaBlue);
                        }
                    }
                }

                
                bmap.UnlockBits(loadedData);
                grayscaleFrame.UnlockBits(processedData);

                
                pictureBox2.Image = grayscaleFrame;
            }
            else if(isButtonClicked == 6)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                Bitmap flippedFrame = new Bitmap(bmap.Width, bmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                var rect = new Rectangle(0, 0, bmap.Width, bmap.Height);
                var sourceData = bmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bmap.PixelFormat);
                var flippedData = flippedFrame.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, flippedFrame.PixelFormat);

                int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(bmap.PixelFormat) / 8;
                int stride = sourceData.Stride;
                IntPtr sourcePtr = sourceData.Scan0;
                IntPtr flippedPtr = flippedData.Scan0;

                unsafe
                {
                    byte* sourcePixel = (byte*)(void*)sourcePtr;
                    byte* flippedPixel = (byte*)(void*)flippedPtr;

                    for (int y = 0; y < bmap.Height; y++)
                    {
                        int sourceY = bmap.Height - 1 - y;
                        byte* sourceRow = sourcePixel + sourceY * stride;
                        byte* flippedRow = flippedPixel + y * stride;

                        for (int x = 0; x < bmap.Width * bytesPerPixel; x++)
                        {
                            flippedRow[x] = sourceRow[x];
                        }
                    }
                }

                bmap.UnlockBits(sourceData);
                flippedFrame.UnlockBits(flippedData);

                pictureBox2.Image = flippedFrame;

            }
            else if(isButtonClicked == 7)

            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                Bitmap flippedFrame = new Bitmap(bmap.Width, bmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                var rect = new Rectangle(0, 0, bmap.Width, bmap.Height);
                var sourceData = bmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bmap.PixelFormat);
                var flippedData = flippedFrame.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, flippedFrame.PixelFormat);

                int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(bmap.PixelFormat) / 8;
                int stride = sourceData.Stride;
                IntPtr sourcePtr = sourceData.Scan0;
                IntPtr flippedPtr = flippedData.Scan0;

                unsafe
                {
                    byte* sourcePixel = (byte*)(void*)sourcePtr;
                    byte* flippedPixel = (byte*)(void*)flippedPtr;

                    for (int y = 0; y < bmap.Height; y++)
                    {
                        byte* sourceRow = sourcePixel + y * stride;
                        byte* flippedRow = flippedPixel + y * stride;

                        for (int x = 0; x < bmap.Width; x++)
                        {
                            int sourceX = bmap.Width - 1 - x;
                            for (int c = 0; c < bytesPerPixel; c++)
                            {
                                flippedRow[x * bytesPerPixel + c] = sourceRow[sourceX * bytesPerPixel + c];
                            }
                        }
                    }
                }

                bmap.UnlockBits(sourceData);
                flippedFrame.UnlockBits(flippedData);

                pictureBox2.Image = flippedFrame;

            }
            else if (isButtonClicked == 8)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                Bitmap scaledimg = null;
                DIPClass.VidScale(ref bmap, ref scaledimg, 50, 50);
                pictureBox2.Image = scaledimg;
            }
            else if(isButtonClicked == 9)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                Bitmap scaledimg = null;
                DIPClass.VidBinaryThreshold(ref bmap, ref scaledimg, 180);
                pictureBox2.Image = scaledimg;
            }
            else if(isButtonClicked == 10)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                Bitmap scaledimg = null;
                int rotationValue;

                if (trackBar3.InvokeRequired)
                {
                    rotationValue = (int)trackBar3.Invoke(new Func<int>(() => trackBar3.Value));
                }
                else
                {
                    rotationValue = trackBar3.Value;
                }
                DIPClass.vidRotate(ref bmap, ref scaledimg, rotationValue);
                pictureBox2.Image = scaledimg;
            }
            else if(isButtonClicked == 11)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                Bitmap scaledimg = null;

                DIPClass.Smooth(bmap, ref scaledimg,1);
                pictureBox2.Image = scaledimg;
            }
            else if (isButtonClicked == 12)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                Bitmap scaledimg = null;

                DIPClass.GaussianBlur(bmap, ref scaledimg, 4);
                pictureBox2.Image = scaledimg;
            }
            else if (isButtonClicked == 13)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                Bitmap scaledimg = null;

                DIPClass.Sharpen(bmap, ref scaledimg, 11);
                pictureBox2.Image = scaledimg;
            }
            else if (isButtonClicked == 14)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                Bitmap scaledimg = null;

                DIPClass.MeanRemoval(bmap, ref scaledimg, 9);
                pictureBox2.Image = scaledimg;
            }
            else if (isButtonClicked == 15)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                Bitmap scaledimg = null;

                DIPClass.Emboss_Laplacian(bmap, ref scaledimg);
                pictureBox2.Image = scaledimg;
            }
            else if (isButtonClicked == 16)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                Bitmap scaledimg = null;

                DIPClass.EmbossHorzVert(bmap, ref scaledimg);
                pictureBox2.Image = scaledimg;
            }
            else if (isButtonClicked == 17)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                Bitmap scaledimg = null;

                DIPClass.Emboss_AllDirection(bmap, ref scaledimg);
                pictureBox2.Image = scaledimg;
            }
            else if (isButtonClicked == 18)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                Bitmap scaledimg = null;

                DIPClass.EmbossLossy(bmap, ref scaledimg);
                pictureBox2.Image = scaledimg;
            }
            else if (isButtonClicked == 19)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                Bitmap scaledimg = null;

                DIPClass.EmbossHorizontalOnly(bmap, ref scaledimg);
                pictureBox2.Image = scaledimg;
            }
            else if (isButtonClicked == 20)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                Bitmap scaledimg = null;

                DIPClass.EmbossVerticalOnly(bmap, ref scaledimg);
                pictureBox2.Image = scaledimg;
            }
            else if (isButtonClicked == 21)
            {
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                BitmapFilter.EdgeDetectQuick(bmap);
                pictureBox2.Image = bmap;
            }
            else if (isButtonClicked == 22)
            {
                //Sobel
                Bitmap bmap = (Bitmap)e.Frame.Clone();
                BitmapFilter.EdgeDetectConvolution(bmap, 3, 75);
                pictureBox2.Image = bmap;
            }
            else if (isButtonClicked == 23)
            {
                //Prewitt

                Bitmap bmap = (Bitmap)e.Frame.Clone();
                BitmapFilter.EdgeDetectConvolution(bmap, 2, 75);
                pictureBox2.Image = bmap;
            }
            else if (isButtonClicked == 24)
            {
                //Kirsh

                Bitmap bmap = (Bitmap)e.Frame.Clone();
                BitmapFilter.EdgeDetectConvolution(bmap, 1, 75);
                pictureBox2.Image = bmap;
            }
            else if (isButtonClicked == 25)
            {

                Bitmap bmap = (Bitmap)e.Frame.Clone();
                BitmapFilter.EdgeDetectHorizontal(bmap);
                pictureBox2.Image = bmap;
            }
            else if (isButtonClicked == 26)
            {

                Bitmap bmap = (Bitmap)e.Frame.Clone();
                BitmapFilter.EdgeDetectVertical(bmap);
                pictureBox2.Image = bmap;
            }
            else if (isButtonClicked == 27)
            {

                Bitmap bmap = (Bitmap)e.Frame.Clone();
                BitmapFilter.EdgeDetectHomogenity(bmap, 75);
                pictureBox2.Image = bmap;
            }
            else if (isButtonClicked == 28)
            {

                Bitmap bmap = (Bitmap)e.Frame.Clone();
                BitmapFilter.EdgeDetectDifference(bmap, 75);
                pictureBox2.Image = bmap;
            }
            else if (isButtonClicked == 29)
            {

                Bitmap bmap = (Bitmap)e.Frame.Clone();
                BitmapFilter.EdgeEnhance(bmap,75);
                pictureBox2.Image = bmap;
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
