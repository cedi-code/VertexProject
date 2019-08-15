using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows;
using System.Linq;

namespace ch03_HelloCube_Net
{
    class BmpG
    {
        private Bitmap bmp;
        private BitmapData data;
        private int stride;
        private SLVec3f color_green = new SLVec3f(0, 255, 0);
        private SLVec3f color_blue = new SLVec3f(0, 0, 255);
        private SLVec3f color_red = new SLVec3f(255, 0, 0);

        public BmpG(int width, int height)
        {
            this.bmp = new Bitmap(width, height);


        }
        

        public void DrawLine(SLVec3f start, SLVec3f end, SLVec3f start_color, SLVec3f end_color)
        {
            List<Point> noPointsNeeded      = new List<Point>();
            List<SLVec3f> noColorListNeeded = new List<SLVec3f>();
            DrawLine(start, end, start_color, end_color, ref noPointsNeeded, ref noColorListNeeded);
        }

        private void DrawLine(SLVec3f start, SLVec3f end, SLVec3f start_color, SLVec3f end_color, ref List<Point> points, ref List<SLVec3f> colors)
        {

            this.data        = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            stride           = data.Stride;
            
            double distance  = 0;


            SLVec3f newColor = new SLVec3f();

            #region setup

            // round values

            RoundVector(ref start);
            RoundVector(ref end);

            int dx           = (int)(end.x - start.x);
            int dy           = (int)(end.y - start.y);


            int stepX = 3, stepY = stride;
            int stepDX = 1, stepDY = 1;


            int maxY = (bmp.Height - 1) * stride, maxX = (bmp.Width - 1) * stepX;

            // längen berechnung
            int posY         = (int) start.y, posX = (int)start.x;
            
            int activeY      = (int)start.y * stride;
            int activeX      = (int)start.x * stepX;

            int lastX = SetLastPos(ref end.x, bmp.Width, stepX);
            int lastY = SetLastPos(ref end.y, bmp.Height, stepY);



            if (dx < 0) { dx = -dx; stepX = -stepX; stepDX = -1; }
            if (dy < 0) { dy = -dy; stepY = -stride; stepDY = -1; }


            double lenght    = Math.Sqrt(Math.Pow(end.x - start.x, 2) + Math.Pow(end.y - start.y, 2));



            #endregion

            unsafe
            {

                byte* ptr = (byte*)data.Scan0;

                if (dx > dy)
                {
                    int dE  = (dy << 1); // dy*2 anstatt dx  / 2
                    int dNE = (dy << 1) - (dx << 1); // dy*2 - dx*2
                    int D   = (dy << 1) - dx; // dy*2 - dx

                    while (activeX != lastX)
                    {

                        if (activeY < maxY && activeX + activeY > 0)
                        {

                            #region color calculation
                            //                                             ___________________________
                            // distance between start and active pixel    √ (x1 - x0)^2 + (y1 - y0)^2
                            distance                   =  Math.Sqrt(  (Math.Pow(posX - start.x,2) + Math.Pow(posY - start.y,2)));
                            // distance percent between the whole line
                            float ratio                = (float)(distance / lenght);
                            if(ratio > 1)              { ratio = 1; }
                            // gradient between start color and end color
                            newColor                   = start_color + ratio * (end_color - start_color);
                            #endregion

                            // set active pixel the rgb (r=x, g=y, b=z)
                            ptr[activeX + activeY]     = (byte)(newColor.z);
                            ptr[activeX + activeY + 1] = (byte)(newColor.y);
                            ptr[activeX + activeY + 2] = (byte)(newColor.x);


                            // saves points to create the primitives
                            points.Add(new Point(posX, posY));
                            colors.Add(newColor);

                        }

                        if (D < 0)
                        {
                            D += dE;
                        }
                        else
                        {
                            D       += dNE;
                            activeY += stepY;
                            posY    += stepDY;
                        }
                        activeX += stepX;
                        posX    += stepDX;
                    }
                }
                else
                {
                    int dE  = (dx << 1);        // = 2*dx
                    int dNE = (dx - dy) << 1;     // = 2*(dx-dy)
                    int e   = (dx << 1) - dy;   // = 2*dx - dy; 

                    while (activeY != lastY)
                    {

                        // todo schon vorher überprüfen ob y oder x null sein könnte
                        if (activeY < maxY && activeX + activeY > 0)
                        {
                            //                                             ___________________________
                            // distance between start and active pixel    √ (x1 - x0)^2 + (y1 - y0)^2
                            distance                   = Math.Sqrt((Math.Pow(posX - start.x, 2) + Math.Pow(posY - start.y, 2)));
                            float ratio                = (float)(distance / lenght);
                            if (ratio > 1)             { ratio = 1; }
                            newColor                   = start_color + ratio * (end_color - start_color);

                            ptr[activeX + activeY]     = (byte)(newColor.z);
                            ptr[activeX + activeY + 1] = (byte)(newColor.y);
                            ptr[activeX + activeY + 2] = (byte)(newColor.x);

                            points.Add(new Point(posX, posY));
                            colors.Add(newColor);

                        }
                        activeY += stepY;
                        posY    += stepDY;

                        if (e < 0)
                        {
                            e += dE;
                        }
                        else
                        {
                            e       += dNE;
                            activeX += stepX;
                            posX    +=stepDX;
                        }

                    }
                }
                // set last pixel 
                if (lastX + lastY > 0)
                {
                    ptr[lastX + lastY]     = (byte)(end_color.z);
                    ptr[lastX + lastY + 1] = (byte)(end_color.y);
                    ptr[lastX + lastY + 2] = (byte)(end_color.x);

                    points.Add(new Point((int)end.x, (int)end.y));
                    colors.Add(end_color);
                }

            }
            bmp.UnlockBits(data);
        } 


        public void DrawPolygon(SLVec3f v0,SLVec3f c0, SLVec3f v1,SLVec3f c1, SLVec3f v2, SLVec3f c2)
        {
            int minX, minY, maxX, maxY;
            List<Point> allPoints = new List<Point>();
            List<SLVec3f> allColors = new List<SLVec3f>();
           

            #region calculate setup

            float[] xPoints = { v0.x, v1.x, v2.x };
            float[] yPoints = { v0.y, v1.y, v2.y };

            minX = (int)xPoints.Min();
            minY = (int)yPoints.Min();

            maxX = (int)Math.Round(xPoints.Max(),0);
            maxY = (int)Math.Round(yPoints.Max(),0);

           

            DrawLine(v0, v1, c0, c1, ref allPoints, ref allColors);
            DrawLine(v1, v2, c1, c2, ref allPoints, ref allColors);
            DrawLine(v2, v0, c2, c0, ref allPoints, ref allColors);

            List<SLVec4f>[] allXonY = new List<SLVec4f>[maxY + 1 - minY];
            List<SLVec4f>[] allSortedXonY = new List<SLVec4f>[maxY + 1 - minY];

            for (int s = 0; s < allXonY.Length; s++)
            {
                allXonY[s] = new List<SLVec4f>();

            }

            SLVec3f vC;
            for (int i = 0; i < allPoints.Count; i++)
            {

                vC = allColors[i];
                allXonY[allPoints[i].Y - minY].Add(new SLVec4f(allPoints[i].X, vC.x, vC.y, vC.z));
            }

            #endregion


            int anzahlX = 0;
            int x1 = bmp.Width;
            int x2 = 0;
            int maxId = 0;
            SLVec3f startC = new SLVec3f();
            SLVec3f endC = new SLVec3f();

            for (int indexY = 0; indexY < allXonY.Length; indexY++)
            {
                // plz fix this D:
                anzahlX = allXonY[indexY].Count;
                if (anzahlX == 0)
                {
                    continue;
                }
                allSortedXonY[indexY] = allXonY[indexY].OrderBy(v => v.x).ToList<SLVec4f>();
                maxId = anzahlX - 1;

                x1 = (int)allSortedXonY[indexY][0].x;
                x2 = (int)allSortedXonY[indexY][maxId].x;
                startC.Set(allSortedXonY[indexY][0].y, allSortedXonY[indexY][0].z, allSortedXonY[indexY][0].w);
                endC.Set(allSortedXonY[indexY][maxId].y, allSortedXonY[indexY][maxId].z, allSortedXonY[indexY][maxId].w);


                draw1DLine(x1, x2, indexY + minY,startC,endC);

                x1 = bmp.Width;
                x2 = 0;
            }



        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"> der linke (kleinere) punkt</param>
        /// <param name="x2"> der rechte (grössere) punkt</param>
        /// <param name="y"></param>
        /// <param name="startColor"></param>
        /// <param name="endColor"></param>
        private void draw1DLine(int x1, int x2, int y, SLVec3f startColor, SLVec3f endColor)
        {
            this.data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            stride = data.Stride;

 

            int stepX = 3, stepY = stride * y, increX = 1;
            int activeX = x1;
            SLVec3f newColor = new SLVec3f();
            //SLVec3f startColor = new SLVec3f();
            //SLVec3f endColor = new SLVec3f();
            double distance, length;
            length = Math.Abs(activeX - x2);




            if (x1 < 0) { x1 = 0; }
            int activePosition = (x1 * stepX) + stepY;

            if(x2 > bmp.Width) { x2 = bmp.Width; }
            int endPosition = x2 * stepX + stepY;

            if (x1 > x2)
            {
                stepX = -stepX;
                increX = -increX;
            }

            unsafe
            {
                byte* ptr = (byte*)data.Scan0;

                //startColor.Set((float)ptr[activePosition], (float)ptr[activePosition + 1], (float)ptr[activePosition + 2]);
                //endColor.Set((float)ptr[endPosition], (float)ptr[endPosition + 1], (float)ptr[endPosition + 2]);
               
                while (activePosition != endPosition)
                {

                    distance = activeX - x1;
                    float ratio = (float)(distance / length);
                    if (ratio > 1) { ratio = 1; }
                    newColor = startColor + ratio * (endColor - startColor);
                    // interpolate!!!!
                    ptr[activePosition] = (byte)newColor.z;
                    ptr[activePosition + 1] = (byte)newColor.y;
                    ptr[activePosition + 2] = (byte)newColor.x;

                    activePosition += stepX;
                    activeX += increX;
                }
                //ptr[endPosition] = (byte)endColor.z;
                //ptr[endPosition + 1] = (byte)endColor.y;
                //ptr[endPosition + 2] = (byte)endColor.x;

            }
            bmp.UnlockBits(data);


        }
        private void checkValues()
        {

        }
        private int SetLastPos(ref float value, int bounds, int multiplier )
        {
            if (value > bounds)
            {
                value = bounds - 1;
                return  (bounds - 1) * multiplier;
                
            }
            else
            {
                return (int)(value) * multiplier;
            }
        }
        /// <summary>
        /// rounds up all values to 0 decimals
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="decimals"></param>
        private void RoundVector(ref SLVec3f vec)
        {
            vec.x = (float)Math.Round(vec.x,0);
            vec.y = (float)Math.Round(vec.y, 0);
            vec.z = (float)Math.Round(vec.z, 0);
        }


        public Bitmap Result()
        {
           
            return bmp;
        }
    }
}
