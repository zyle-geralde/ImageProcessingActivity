using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessingActivity
{
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

            for (int x = 0; x <256; x++)
            {
                for (int y = 0; y < Math.Min(histdata[x]/5,proc.Height-1); y++)
                {
                    proc.SetPixel(x, (proc.Height - 1)-y, Color.Black);
                }
            }
        }
    }
}
