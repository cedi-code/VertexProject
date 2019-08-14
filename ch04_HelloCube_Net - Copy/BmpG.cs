using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows;

namespace ch03_HelloCube_Net
{
    class BmpG
    {
        private Bitmap bmp;
        private BitmapData data;
        private int stride;

        public BmpG(int width, int height)
        {
            this.bmp = new Bitmap(width, height);


        }
        

        public void DrawLine(SLVec3f start, SLVec3f end, SLVec3f start_color, SLVec3f end_color)
        {

            this.data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            stride = data.Stride;
            
            double distance = 0;

            SLVec3f newColor = new SLVec3f();

            #region setup

            int dx = (int)(end.x - start.x);
            int dy = (int)(end.y - start.y);


            int stepX = 3, stepY = stride;

            // TODO zu viele Variablen!! bounds befor draw line überprüfen damit es nicht für jede line / pixel überprüft werden muss!!
            int maxX = bmp.Width * stepX, maxY = bmp.Height * stride;

            // längen berechnung
            int posY = (int) start.y, posX = (int)start.x;
            int activeY = (int)start.y * stride;
            int activeX = (int)start.x * stepX;

            int lastX = 0;
            int lastY = 0;
            if ((int)end.x > bmp.Width) { lastX = maxX; } else { lastX = (int)(end.x) * stepX; }

            if ((int)end.y > bmp.Height) { lastY = maxY; } else { lastY = (int)(end.y) * stride; }

            if (dx < 0) { dx = -dx; stepX = -stepX; }
            if (dy < 0) { dy = -dy; stepY = -stride; }

            double lenght = Math.Sqrt(Math.Pow(end.x - start.x, 2) + Math.Pow(end.y - start.y, 2));
            #endregion

            unsafe
            {

                byte* ptr = (byte*)data.Scan0;

                if (dx > dy)
                {
                    int dE = (dy << 1); // dy*2 anstatt dx  / 2
                    int dNE = (dy << 1) - (dx << 1); // dy*2 - dx*2
                    int D = (dy << 1) - dx; // dy*2 - dx


                    // ptr[activeX + activeY + 3] = m_colour.A;
                    while (activeX != lastX)
                    {

                        if (activeY < maxY && activeX + activeY > 0)
                        {
                            
                            distance                   = Math.Sqrt(  (Math.Pow(posX - start.x,2) + Math.Pow(posY - start.y,2)));
                            float ratio                = (float)(distance / lenght);
                            newColor                   = start_color + ratio * (end_color - start_color);

                            ptr[activeX + activeY]     = Convert.ToByte(newColor.z);
                            ptr[activeX + activeY + 1] = Convert.ToByte(newColor.y);
                            ptr[activeX + activeY + 2] = Convert.ToByte(newColor.x);
                        }

                        if (D < 0)
                        {
                            D += dE;
                        }
                        else
                        {
                            D += dNE;
                            activeY += stepY;
                            posY++;
                        }
                        activeX += stepX;
                        posX++;


                    }
                }
                else
                {
                    int dE = (dx << 1);        // = 2*dx
                    int dNE = (dx - dy) << 1;     // = 2*(dx-dy)
                    int e = (dx << 1) - dy;   // = 2*dx - dy; 

                    while (activeY != lastY)
                    {



                        if (activeY < maxY && activeX + activeY > 0)
                        {
                            distance = Math.Sqrt((Math.Pow(posX - start.x, 2) + Math.Pow(posY - start.y, 2)));
                            float ratio = (float)(distance / lenght);
                            newColor = start_color + ratio * (end_color - start_color);

                            ptr[activeX + activeY] = Convert.ToByte(newColor.z);
                            ptr[activeX + activeY + 1] = Convert.ToByte(newColor.y);
                            ptr[activeX + activeY + 2] = Convert.ToByte(newColor.x);
                        }

                        activeY += stepY;
                        posY++;
                        if (e < 0)
                        {
                            e += dE;
                        }
                        else
                        {
                            e += dNE;
                            activeX += stepX;
                            posX++;
                        }
                    }
                }

                // last pixel 
                if (lastX + lastY > 0)
                {
                    distance = Math.Sqrt((Math.Pow(posX - start.x, 2) + Math.Pow(posY - start.y, 2)));
                    float ratio = (float)(distance / lenght);
                    newColor = start_color + ratio * (end_color - start_color);

                    ptr[lastX + lastY] = Convert.ToByte(newColor.z);
                    ptr[lastX + lastY + 1] = Convert.ToByte(newColor.y);
                    ptr[lastX + lastY + 2] = Convert.ToByte(newColor.x);
                }




            }
            bmp.UnlockBits(data);


        } 


        public void DrawPolygon(SLVec3f v0, SLVec3f v1, SLVec3f v2)
        {
            int minX, minY, maxX, maxY;

            #region calculate setup

            float[] xPoints = { v0.x, v1.x, v2.x };
            float[] yPoints = { v0.y, v1.y, v2.y };

            minX = (int)xPoints.Min(); // todo u.s.w
            #endregion
        }
        public Bitmap Result()
        {
           
            return bmp;
        }
    }
}
