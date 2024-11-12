using ImageProcess2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessingActivity
{
    public class ConvMatrix
    {
        public int TopLeft = 0, TopMid = 0, TopRight = 0;
        public int MidLeft = 0, Pixel = 1, MidRight = 0;
        public int BottomLeft = 0, BottomMid = 0, BottomRight = 0;
        public int Factor = 1;
        public int Offset = 0;
        public void SetAll(int nVal)
        {
            TopLeft = TopMid = TopRight = MidLeft = Pixel = MidRight = BottomLeft = BottomMid = BottomRight = nVal;
        }
    }
    static class DIPClass
    {
        public static void Histogram(ref Bitmap load, ref Bitmap proc)
        {
            Color sample;
            Color gray;
            Byte greydata;

            for (int x = 0; x < load.Width; x++)
            {
                for (int y = 0; y < load.Height; y++)
                {
                    sample = load.GetPixel(x, y);
                    greydata = (byte)((sample.R + sample.G + sample.B) / 3);
                    gray = Color.FromArgb(greydata, greydata, greydata);
                    load.SetPixel(x, y, gray);
                }
            }

            int[] histdata = new int[256];
            for (int x = 0; x < load.Width; x++)
            {
                for (int y = 0; y < load.Height; y++)
                {
                    sample = load.GetPixel(x, y);
                    histdata[sample.R]++;
                }
            }

            proc = new Bitmap(256, 800);
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 800; y++)
                {
                    proc.SetPixel(x, y, Color.White);
                }
            }

            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < Math.Min(histdata[x] / 5, proc.Height - 1); y++)
                {
                    proc.SetPixel(x, (proc.Height - 1) - y, Color.Black);
                }
            }
        }

        public static void Equalisation(Bitmap a, ref Bitmap b, int degree)
        {
            int height = a.Height;
            int width = a.Width;
            int numSamples, histSum;
            int[] Ymap = new int[256];
            int[] hist = new int[256];
            int percent = degree;

            Color calculated;
            Color gray;
            Byte graydata;
            for (int x = 0; x < a.Width; x++)
            {
                for (int y = 0; y < a.Height; y++)
                {
                    calculated = a.GetPixel(x, y);
                    graydata = (byte)((calculated.R + calculated.G + calculated.B) / 3);
                    hist[graydata]++;
                }
            }
            numSamples = (width * height);
            histSum = 0;
            for (int h = 0; h < 256; h++)
            {
                histSum += hist[h];
                Ymap[h] = histSum * 255 / numSamples;
            }

            if (percent < 100)
            {
                for (int h = 0; h < 256; h++)
                {
                    Ymap[h] = h + ((int)Ymap[h] - h) * percent / 100;
                }
            }

            b = new Bitmap(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color temp = Color.FromArgb(Ymap[a.GetPixel(x, y).R], Ymap[a.GetPixel(x, y).G], Ymap[a.GetPixel(x, y).B]);
                    b.SetPixel(x, y, temp);
                }
            }
        }
        public static void Rotate(Bitmap sourceImage, ref Bitmap rotatedImage, int angleDegrees)
        {
            float angleRadians = (float)(Math.PI * angleDegrees / 180.0);  
            int centerX = sourceImage.Width / 2;
            int centerY = sourceImage.Height / 2;
            int width = sourceImage.Width;
            int height = sourceImage.Height;

            rotatedImage = new Bitmap(width, height); 

            float cosAngle = (float)Math.Cos(angleRadians);
            float sinAngle = (float)Math.Sin(angleRadians);

            for (int newX = 0; newX < width; newX++)
            {
                for (int newY = 0; newY < height; newY++)
                {
                    int offsetX = newX - centerX;  
                    int offsetY = newY - centerY;  

                    
                    int originalX = (int)(offsetX * cosAngle + offsetY * sinAngle) + centerX;
                    int originalY = (int)(-offsetX * sinAngle + offsetY * cosAngle) + centerY;

                    
                    originalX = Math.Max(0, Math.Min(width - 1, originalX));
                    originalY = Math.Max(0, Math.Min(height - 1, originalY));

                    rotatedImage.SetPixel(newX, newY, sourceImage.GetPixel(originalX, originalY));
                }
            }
        }

        public static void Scale(ref Bitmap sourceImage, ref Bitmap scaledImage, int targetWidth, int targetHeight)
        {
            int width = sourceImage.Width;
            int height = sourceImage.Height;
            scaledImage = new Bitmap(targetWidth, targetHeight);

            for (int targetX = 0; targetX < targetWidth; targetX++)
            {
                for (int targetY = 0; targetY < targetHeight; targetY++)
                {
                    int sourceX = targetX * width / targetWidth;
                    int sourceY = targetY * height / targetHeight;
                    scaledImage.SetPixel(targetX, targetY, sourceImage.GetPixel(sourceX, sourceY));
                }
            }
        }

        public static void BinaryThreshold(ref Bitmap sourceImage, ref Bitmap binaryImage, int thresholdValue)
        {
            if (sourceImage == null)
                return;

            binaryImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            int threshold = thresholdValue;

            for (int x = 0; x < sourceImage.Width; ++x)
            {
                for (int y = 0; y < sourceImage.Height; ++y)
                {
                    Color pixel = sourceImage.GetPixel(x, y);
                    int grayscale = (pixel.R + pixel.G + pixel.B) / 3;
                    binaryImage.SetPixel(x, y, grayscale < threshold ? Color.Black : Color.White);
                }
            }
        }


        public static void VidScale(ref Bitmap sourceImage, ref Bitmap scaledImage, int targetWidth, int targetHeight)
        {
            scaledImage = new Bitmap(targetWidth, targetHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var rectSource = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
            var rectTarget = new Rectangle(0, 0, targetWidth, targetHeight);

            var sourceData = sourceImage.LockBits(rectSource, System.Drawing.Imaging.ImageLockMode.ReadOnly, sourceImage.PixelFormat);
            var targetData = scaledImage.LockBits(rectTarget, System.Drawing.Imaging.ImageLockMode.WriteOnly, scaledImage.PixelFormat);

            int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(sourceImage.PixelFormat) / 8;
            int sourceStride = sourceData.Stride;
            int targetStride = targetData.Stride;

            IntPtr sourcePtr = sourceData.Scan0;
            IntPtr targetPtr = targetData.Scan0;

            unsafe
            {
                byte* sourcePixel = (byte*)(void*)sourcePtr;
                byte* targetPixel = (byte*)(void*)targetPtr;

                for (int targetY = 0; targetY < targetHeight; targetY++)
                {
                    int sourceY = targetY * sourceImage.Height / targetHeight;
                    byte* sourceRow = sourcePixel + sourceY * sourceStride;
                    byte* targetRow = targetPixel + targetY * targetStride;

                    for (int targetX = 0; targetX < targetWidth; targetX++)
                    {
                        int sourceX = targetX * sourceImage.Width / targetWidth;
                        byte* targetPixelData = targetRow + targetX * bytesPerPixel;
                        byte* sourcePixelData = sourceRow + sourceX * bytesPerPixel;

                        for (int c = 0; c < bytesPerPixel; c++)
                        {
                            targetPixelData[c] = sourcePixelData[c];
                        }
                    }
                }
            }

            sourceImage.UnlockBits(sourceData);
            scaledImage.UnlockBits(targetData);
        }

        public static void VidBinaryThreshold(ref Bitmap sourceImage, ref Bitmap binaryImage, int thresholdValue)
        {
            if (sourceImage == null)
                return;

            binaryImage = new Bitmap(sourceImage.Width, sourceImage.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var rect = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
            var sourceData = sourceImage.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, sourceImage.PixelFormat);
            var binaryData = binaryImage.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, binaryImage.PixelFormat);

            int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(sourceImage.PixelFormat) / 8;
            int stride = sourceData.Stride;
            IntPtr sourcePtr = sourceData.Scan0;
            IntPtr binaryPtr = binaryData.Scan0;

            unsafe
            {
                byte* sourcePixel = (byte*)(void*)sourcePtr;
                byte* binaryPixel = (byte*)(void*)binaryPtr;

                for (int y = 0; y < sourceImage.Height; y++)
                {
                    byte* sourceRow = sourcePixel + y * stride;
                    byte* binaryRow = binaryPixel + y * stride;

                    for (int x = 0; x < sourceImage.Width; x++)
                    {
                        int index = x * bytesPerPixel;
                        int grayscale = (sourceRow[index] + sourceRow[index + 1] + sourceRow[index + 2]) / 3;
                        byte colorValue = (byte)(grayscale < thresholdValue ? 0 : 255);

                        binaryRow[index] = colorValue;       
                        binaryRow[index + 1] = colorValue;   
                        binaryRow[index + 2] = colorValue;   
                    }
                }
            }

            sourceImage.UnlockBits(sourceData);
            binaryImage.UnlockBits(binaryData);
        }

        public static void vidRotate(ref Bitmap sourceImage, ref Bitmap rotatedImage, int angleDegrees)
        {
            float angleRadians = (float)(Math.PI * angleDegrees / 180.0);
            int centerX = sourceImage.Width / 2;
            int centerY = sourceImage.Height / 2;
            int width = sourceImage.Width;
            int height = sourceImage.Height;

            rotatedImage = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            float cosAngle = (float)Math.Cos(angleRadians);
            float sinAngle = (float)Math.Sin(angleRadians);

            var rect = new Rectangle(0, 0, width, height);
            var sourceData = sourceImage.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, sourceImage.PixelFormat);
            var rotatedData = rotatedImage.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, rotatedImage.PixelFormat);

            int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(sourceImage.PixelFormat) / 8;
            int stride = sourceData.Stride;
            IntPtr sourcePtr = sourceData.Scan0;
            IntPtr rotatedPtr = rotatedData.Scan0;

            unsafe
            {
                byte* sourcePixel = (byte*)(void*)sourcePtr;
                byte* rotatedPixel = (byte*)(void*)rotatedPtr;

                for (int newY = 0; newY < height; newY++)
                {
                    byte* rotatedRow = rotatedPixel + newY * stride;

                    for (int newX = 0; newX < width; newX++)
                    {
                        int offsetX = newX - centerX;
                        int offsetY = newY - centerY;

                        int originalX = (int)(offsetX * cosAngle + offsetY * sinAngle) + centerX;
                        int originalY = (int)(-offsetX * sinAngle + offsetY * cosAngle) + centerY;

                        if (originalX >= 0 && originalX < width && originalY >= 0 && originalY < height)
                        {
                            byte* sourceRow = sourcePixel + originalY * stride;
                            byte* sourcePixelData = sourceRow + originalX * bytesPerPixel;
                            byte* rotatedPixelData = rotatedRow + newX * bytesPerPixel;

                            for (int c = 0; c < bytesPerPixel; c++)
                            {
                                rotatedPixelData[c] = sourcePixelData[c];
                            }
                        }
                    }
                }
            }

            sourceImage.UnlockBits(sourceData);
            rotatedImage.UnlockBits(rotatedData);
        }




        /*--------------------CONVOLUTIONAL SECTION--------------------------*/

        public static Bitmap Conv3x3(Bitmap b, ConvMatrix m)
        {
            // Avoid divide by zero errors
            if (0 == m.Factor || b == null) return null;

            Bitmap bSrc = (Bitmap)b.Clone();
            Bitmap r = new Bitmap(bSrc.Width, bSrc.Height);

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = r.LockBits(new Rectangle(0, 0, r.Width, r.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            int stride2 = stride * 2;
            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr SrcScan0 = bmSrc.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pSrc = (byte*)(void*)SrcScan0;

                int nOffset = stride - b.Width * 3;
                int nWidth = b.Width - 2;
                int nHeight = b.Height - 2;

                int nPixel;

                for (int y = 0; y < nHeight; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {
                        nPixel = ((((pSrc[2] * m.TopLeft) + (pSrc[5] * m.TopMid) + (pSrc[8] * m.TopRight) +
                            (pSrc[2 + stride] * m.MidLeft) + (pSrc[5 + stride] * m.Pixel) + (pSrc[8 + stride] * m.MidRight) +
                            (pSrc[2 + stride2] * m.BottomLeft) + (pSrc[5 + stride2] * m.BottomMid) + (pSrc[8 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[5 + stride] = (byte)nPixel;

                        nPixel = ((((pSrc[1] * m.TopLeft) + (pSrc[4] * m.TopMid) + (pSrc[7] * m.TopRight) +
                            (pSrc[1 + stride] * m.MidLeft) + (pSrc[4 + stride] * m.Pixel) + (pSrc[7 + stride] * m.MidRight) +
                            (pSrc[1 + stride2] * m.BottomLeft) + (pSrc[4 + stride2] * m.BottomMid) + (pSrc[7 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[4 + stride] = (byte)nPixel;

                        nPixel = ((((pSrc[0] * m.TopLeft) + (pSrc[3] * m.TopMid) + (pSrc[6] * m.TopRight) +
                            (pSrc[0 + stride] * m.MidLeft) + (pSrc[3 + stride] * m.Pixel) + (pSrc[6 + stride] * m.MidRight) +
                            (pSrc[0 + stride2] * m.BottomLeft) + (pSrc[3 + stride2] * m.BottomMid) + (pSrc[6 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[3 + stride] = (byte)nPixel;

                        p += 3;
                        pSrc += 3;
                    }
                    p += nOffset;
                    pSrc += nOffset;
                }
            }

            r.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);

            return r;
        }

        public static void Smooth(Bitmap b,ref Bitmap r, int nWeight /* default to 1 */)
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(1);
            m.Pixel = nWeight;
            m.Factor = nWeight + 8;
            r = Conv3x3(b, m);
        }


        public static void Sharpen(Bitmap b, ref Bitmap r,int nWeight /* default to 11*/ )
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(0);
            m.Pixel = nWeight;
            m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = -2;
            m.Factor = nWeight - 8;

            r = Conv3x3(b, m);
        }

        public static void GaussianBlur(Bitmap b, ref Bitmap r, int nWeight /* default to 4*/)
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(1);
            m.Pixel = nWeight;
            m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = 2;
            m.Factor = nWeight + 12;

            r = Conv3x3(b, m);
        }

        public static void MeanRemoval(Bitmap b, ref Bitmap r,int nWeight /* default to 9*/ )
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(-1);
            m.Pixel = nWeight;
            m.Factor = nWeight - 8;

            r = Conv3x3(b, m);
        }

        public static void Emboss_Laplacian(Bitmap b,ref Bitmap r)
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(-1);
            m.MidLeft = m.TopMid = m.BottomMid  = m.MidRight = 0;
            m.Pixel = 4;
            m.Offset = 127;

            r = Conv3x3(b, m);
        }

        public static void Emboss_AllDirection(Bitmap b, ref Bitmap r)
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(-1);
            m.Pixel = 8;
            m.Offset = 127;

            r= Conv3x3(b, m);
        }

        public static void EmbossLossy(Bitmap b, ref Bitmap r)
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(-2);
            m.TopLeft = m.BottomMid = m.TopRight = 1;
            m.Pixel = 4;
            m.Offset = 127;

            r = Conv3x3(b, m);
        }

        public static void EmbossHorzVert(Bitmap b, ref Bitmap r)
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(0);
            m.MidLeft = m.TopMid = m.BottomMid = m.MidRight = -1;
            m.Pixel = 4;
            m.Offset = 127;

            r = Conv3x3(b, m);
        }

        public static void EmbossVerticalOnly(Bitmap b, ref Bitmap r)
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(0);
            m.TopMid = -1;
            m.BottomMid = 1;
            m.Pixel = 0;
            m.Offset = 127;

            r = Conv3x3(b, m);
        }

        public static void EmbossHorizontalOnly(Bitmap b, ref Bitmap r)
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(0);
            m.MidLeft = m.MidRight = -1;
            m.Pixel = 2;
            m.Offset = 127;

            r = Conv3x3(b, m);
        }
    }
}
