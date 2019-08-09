using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace ch03_HelloCube_Net
{
    class cg
    {
        private Bitmap bmp;
        private BitmapData data;
        private int stride;

        public cg(int width, int height)
        {
            this.bmp = new Bitmap(width, height);
            this.data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = data.Stride; // width of lockt bitmap??
        }

        // public void set
    }
}
