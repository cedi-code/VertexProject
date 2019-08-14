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
        private SLVec3f color_green = new SLVec3f(0, 255, 0);
        private SLVec3f color_blue = new SLVec3f(0, 0, 255);
        private SLVec3f color_red = new SLVec3f(255, 0, 0);

        public BmpG(int width, int height)
        {
            this.bmp = new Bitmap(width, height);


        }
        

        public void DrawLine(SLVec3f start, SLVec3f end, SLVec3f start_color, SLVec3f end_color)
        {
            List<Point> noPointsNeeded = new List<Point>();
            List<SLVec3f> noColorListNeeded = new List<SLVec3f>();
            DrawLine(start, end, start_color, end_color, ref noPointsNeeded, ref noColorListNeeded);
        }

        private void DrawLine(SLVec3f start, SLVec3f end, SLVec3f start_color, SLVec3f end_color, ref List<Point> points, ref List<SLVec3f> colors)
        {

            this.data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            stride = data.Stride;
            
            double distance = 0;


            SLVec3f newColor = new SLVec3f();

            #region setup

            int dx = (int)(end.x - start.x);
            int dy = (int)(end.y - start.y);


            int stepX = 3, stepY = stride, stepDX = 1, stepDY = 1 ;

            // TODO zu viele Variablen!! bounds befor draw line überprüfen damit es nicht für jede line / pixel überprüft werden muss!!
            int maxX = (bmp.Width-1) * stepX, maxY = (bmp.Height-1) * stride;

            // längen berechnung
            int posY = (int) start.y, posX = (int)start.x;
            int activeY = (int)start.y * stride;
            int activeX = (int)start.x * stepX;

            int lastX = 0;
            int lastY = 0;
            if (end.x > bmp.Width) { lastX = maxX; end.x = bmp.Width-1; } else { lastX = (int)(end.x) * stepX; }

            if (end.y > bmp.Height) { lastY = maxY; end.y = bmp.Height-1; } else { lastY = (int)(end.y) * stride; }

            if (dx < 0) { dx = -dx; stepX = -stepX; stepDX = -1; }
            if (dy < 0) { dy = -dy; stepY = -stride; stepDY = -1; }

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
                            if(ratio > 1) { ratio = 1; }
                            newColor                   = start_color + ratio * (end_color - start_color);

                            ptr[activeX + activeY]     = (byte)(newColor.z);
                            ptr[activeX + activeY + 1] = (byte)(newColor.y);
                            ptr[activeX + activeY + 2] = (byte)(newColor.x);


                            // new oder auf ein point immer ändern pos.X = posX
                            points.Add(new Point(posX, posY));
                            colors.Add(newColor);
                        }

                        if (D < 0)
                        {
                            D += dE;
                        }
                        else
                        {
                            D += dNE;
                            activeY += stepY;
                            posY += stepDY;
                        }
                        activeX += stepX;
                        posX += stepDX;


                    }
                }
                else
                {
                    int dE = (dx << 1);        // = 2*dx
                    int dNE = (dx - dy) << 1;     // = 2*(dx-dy)
                    int e = (dx << 1) - dy;   // = 2*dx - dy; 

                    while (activeY != lastY)
                    {


                        // todo schon vorher überprüfen ob y oder x null sein könnte
                        if (activeY < maxY && activeX + activeY > 0)
                        {
                            distance = Math.Sqrt((Math.Pow(posX - start.x, 2) + Math.Pow(posY - start.y, 2)));
                            float ratio = (float)(distance / lenght);
                            if (ratio > 1) { ratio = 1; }
                            newColor = start_color + ratio * (end_color - start_color);

                            ptr[activeX + activeY] = (byte)(newColor.z);
                            ptr[activeX + activeY + 1] = (byte)(newColor.y);
                            ptr[activeX + activeY + 2] = (byte)(newColor.x);

                            points.Add(new Point(posX, posY));
                            colors.Add(newColor);
                        }

                        activeY += stepY;
                        posY += stepDY;
                        if (e < 0)
                        {
                            e += dE;
                        }
                        else
                        {
                            e += dNE;
                            activeX += stepX;
                            posX +=stepDX;
                        }
                    }
                }

                // last pixel 
                if (lastX + lastY > 0)
                {


                    ptr[lastX + lastY] = (byte)(end_color.z);
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

            minX = (int)xPoints.Min(); // todo u.s.w
            minY = (int)yPoints.Min();

            maxX = (int)xPoints.Max();
            maxY = (int)yPoints.Max();


            DrawLine(v0, v1, c0, c1, ref allPoints, ref allColors);
            DrawLine(v1, v2, c1, c2, ref allPoints, ref allColors);
            DrawLine(v2, v0, c2, c0, ref allPoints, ref allColors);

            List<SLVec4f>[] allXonY = new List<SLVec4f>[maxY + 1 - minY];
            for(int s = 0; s < allXonY.Length; s++)
            {
                allXonY[s] = new List<SLVec4f>();
            }

            SLVec3f fC;
            for (int i = 0; i < allPoints.Count; i++)
            {

                fC = allColors[i];
                allXonY[allPoints[i].Y - minY].Add(new SLVec4f(allPoints[i].X, fC.x, fC.y, fC.z));
            }

            #endregion
            // TODO bei Y nicht vergessen + minY zu rechnen, da es ja auf 0 gesetz wurde!
            int anzahlX = 0;
            int x1 = bmp.Width;
            int x2 = 0;
            int maxIndex = 0;
            int minIndex = 0;
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
                //if(anzahlX > 2)
                //{
                for (int ugly = 0; ugly < anzahlX; ugly++)
                {
                    if (allXonY[indexY][ugly].x < x1) { x1 = (int)allXonY[indexY][ugly].x; minIndex = ugly; }
                    if (allXonY[indexY][ugly].x > x2) { x2 = (int)allXonY[indexY][ugly].x; maxIndex = ugly; }
                }
                startC.Set(allXonY[indexY][minIndex].y, allXonY[indexY][minIndex].z, allXonY[indexY][minIndex].w);
                endC.Set(allXonY[indexY][maxIndex].y, allXonY[indexY][maxIndex].z, allXonY[indexY][maxIndex].w);
                //}
                //else
                //{
                //    x1 = (int)allXonY[indexY][0].x;
                //    x2 = (int)allXonY[indexY][anzahlX - 1].x;
                //    startC.Set(allXonY[indexY][0].y, allXonY[indexY][0].z, allXonY[indexY][0].w);
                //    endC.Set(allXonY[indexY][anzahlX - 1].y, allXonY[indexY][anzahlX - 1].z, allXonY[indexY][anzahlX - 1].w);
                //}


                drawStraightLine(x1, x2, indexY + minY, startC, endC);

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
        private void drawStraightLine(int x1, int x2, int y,SLVec3f startColor, SLVec3f endColor)
        {
            this.data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            stride = data.Stride;

 

            int stepX = 3, stepY = stride * y, increX = 1;
            int activeX = x1;
            SLVec3f newColor = new SLVec3f();
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
            }
            bmp.UnlockBits(data);


        }


        public Bitmap Result()
        {
           
            return bmp;
        }
    }
}
