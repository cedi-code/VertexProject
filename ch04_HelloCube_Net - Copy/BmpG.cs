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
        #region values
        private Bitmap bmp;
        private BitmapData data;
        private int stride;
        private float[,] zBuffer;
        #endregion

        /// <summary>
        /// Creates a Bitmap with set height and width
        /// </summary>
        /// <param name="width">Width of the bitmap</param>
        /// <param name="height">Height of the bitmap</param>
        public BmpG(int width, int height)
        {
            this.bmp = new Bitmap(width, height);
            zBuffer = new float[height,width];
            for(int h = 0; h < height-1; h++)
            {
                for(int w = 0; w < width -1; w++)
                {
                    zBuffer[h,w] = 9999.99f;
                }
            }

        }
        
        /// <summary>
        /// Draws Line in the Bitmap
        /// </summary>
        /// <param name="start">starting vector</param>
        /// <param name="end">ending vector</param>
        /// <param name="start_color">starting color of starting vector</param>
        /// <param name="end_color">ending color of ending vector</param>
        public void DrawLine(SLVec3f start, SLVec3f end, SLVec3f start_color, SLVec3f end_color)
        {
            List<SLVec3f> noPointsNeeded      = new List<SLVec3f>();
            List<SLVec3f> noColorListNeeded = new List<SLVec3f>();
            DrawLine(start, end, start_color, end_color, ref noPointsNeeded, ref noColorListNeeded);
        }

        /// <summary>
        /// Draws Line in the Bitmap and sets up the Primitives
        /// </summary>
        /// <param name="start">starting vector</param>
        /// <param name="end">ending vector</param>
        /// <param name="start_color">starting color of starting vector</param>
        /// <param name="end_color">ending color of ending vector</param>
        /// <param name="points">saves the coordinates of the line</param>
        /// <param name="colors">saves the colors of the line</param>
        private void DrawLine(SLVec3f start, SLVec3f end, SLVec3f start_color, SLVec3f end_color, ref List<SLVec3f> points, ref List<SLVec3f> colors)
        {


            #region setup

            this.data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            stride = data.Stride;

            // color calculations
            double distance = 0;
            SLVec3f newColor = new SLVec3f();

            // round values
            RoundVector(ref start);
            RoundVector(ref end);

            // calculates slopes
            int dx           = (int)(end.x - start.x);
            int dy           = (int)(end.y - start.y);

            // steps size for calculation in ptr array
            int stepX = 3,   stepY = stride;
            int stepDX = 1,  stepDY = 1;

            // positions to know the coordinates of the active position
            int posY         = (int)start.y;
            int posX         = (int)start.x;
            float posZ;
            float startZ     = start.z;
            float endZ       = end.z;
            
            // transformed starting coordinates for the ptr array
            int activeY      = (int)start.y * stride;
            int activeX      = (int)start.x * stepX;

            // transformed ending coordinates for the ptr array
            int lastX        = (int)end.x * stepX;
            int lastY        =  (int)end.y * stepY;

            // if the slopes are negative
            if (dx < 0)
            {
                invertValues(ref dx, ref stepX, ref stepDX);
            }
            if (dy < 0)
            {
                invertValues(ref dy, ref stepY, ref stepDY);
            }

            //                                             ___________________________
            // distance between start and end vector      √ (x1 - x0)^2 + (y1 - y0)^2
            double lenght    = Math.Sqrt(Math.Pow(end.x - start.x, 2) + Math.Pow(end.y - start.y, 2));



            #endregion

            unsafe
            {
                // gets the first pixel adress in the bitmap
                byte* ptr = (byte*)data.Scan0;

                if (dx > dy)
                {
                    int dE  = (dy << 1); // dy*2 anstatt dx  / 2
                    int dNE = (dy << 1) - (dx << 1); // dy*2 - dx*2
                    int D   = (dy << 1) - dx; // dy*2 - dx

                    while (activeX != lastX)
                    {

                        

                        #region color calculation
                        //                                             ___________________________
                        // distance between start and active pixel    √ (x1 - x0)^2 + (y1 - y0)^2
                        distance                   =  Math.Sqrt(  (Math.Pow(posX - start.x,2) + Math.Pow(posY - start.y,2)));
                        // distance percent between the whole line
                        float ratio                = (float)(distance / lenght);
                        if(ratio > 1)              { ratio = 1; }
                        // gradient between start color and end color
                        newColor = start_color + ratio * (end_color - start_color);

                        #endregion
                        posZ = startZ + ratio * (endZ - startZ);
                        if (zBuffer[posY, posX] > posZ)
                        {
                            zBuffer[posY, posX] = posZ;

                            // set active pixel the rgb (r=x, g=y, b=z)
                            ptr[activeX + activeY] = (byte)(newColor.z);
                            ptr[activeX + activeY + 1] = (byte)(newColor.y);
                            ptr[activeX + activeY + 2] = (byte)(newColor.x);
                        }



                        // saves points to create the primitives
                        points.Add(new SLVec3f(posX, posY, posZ));
                        colors.Add(newColor);
                        



                        if (D < 0)
                        {
                            D += dE;
                        }
                        else
                        {
                            D       += dNE;
                            activeY += stepY;
                            posY += stepDY;
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

                        #region color calculation
                        //                                             ___________________________
                        // distance between start and active pixel    √ (x1 - x0)^2 + (y1 - y0)^2
                        distance = Math.Sqrt((Math.Pow(posX - start.x, 2) + Math.Pow(posY - start.y, 2)));
                        float ratio                = (float)(distance / lenght);
                        if (ratio > 1)             { ratio = 1; }
                        newColor                   = start_color + ratio * (end_color - start_color);
                        #endregion
                        posZ = startZ + ratio * (endZ - startZ);
                        if (zBuffer[posY, posX] > posZ)
                        {
                            zBuffer[posY, posX] = posZ;
                            // set active pixel the rgb (r=x, g=y, b=z)
                            ptr[activeX + activeY] = (byte)(newColor.z);
                            ptr[activeX + activeY + 1] = (byte)(newColor.y);
                            ptr[activeX + activeY + 2] = (byte)(newColor.x);
                        }

                        // saves points to create the primitives
                        points.Add(new SLVec3f(posX,posY,posZ));
                        colors.Add(newColor);

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
                if(zBuffer[posY, posX] < end.z)
                {
                    zBuffer[posY, posX] = end.z;
                    ptr[lastX + lastY] = (byte)(end_color.z);
                    ptr[lastX + lastY + 1] = (byte)(end_color.y);
                    ptr[lastX + lastY + 2] = (byte)(end_color.x);
                }
  
                points.Add(end);
                colors.Add(end_color);

            }
            bmp.UnlockBits(data);
        }


        /// <summary>
        /// Draws an fill in a Polygon counterclockwise
        /// </summary>
        /// <param name="v0">first vector</param>
        /// <param name="c0">color from vector 0 </param>
        /// <param name="v1">second vector</param>
        /// <param name="c1">color from vector 1 </param>
        /// <param name="v2">third and last vector</param>
        /// <param name="c2">color4 from vector 2 </param>
        public void DrawPolygon(SLVec3f v0,SLVec3f c0, SLVec3f v1,SLVec3f c1, SLVec3f v2, SLVec3f c2)
        {
            #region values
            int minX, minY, maxX, maxY;
            List<SLVec3f> allPoints = new List<SLVec3f>();
            List<SLVec3f> allColors = new List<SLVec3f>();

            float[] xPoints = { v0.x, v1.x, v2.x };
            float[] yPoints = { v0.y, v1.y, v2.y };

            int anzahlX = 0;
            int x1 = bmp.Width;
            int x2 = 0;
            int maxId = 0;
            SLVec3f startC = new SLVec3f();
            SLVec3f endC = new SLVec3f();

            #endregion

            #region calculate setup

            // calculates the square around the triangle
            minX            = (int)xPoints.Min();
            minY            = (int)yPoints.Min();

            maxX            = (int)Math.Round(xPoints.Max(),0);
            maxY            = (int)Math.Round(yPoints.Max(),0);

           
            // Draws the line between the 3 vectors and saves the primitves
            DrawLine(v0, v1, c0, c1, ref allPoints, ref allColors);
            DrawLine(v1, v2, c1, c2, ref allPoints, ref allColors);
            DrawLine(v2, v0, c2, c0, ref allPoints, ref allColors);

            // creates list on the size of the square (boundries)
            List<SL2Vec3f>[] allXonY       = new List<SL2Vec3f>[maxY + 1 - minY];
            List<SL2Vec3f>[] allSortedXonY = new List<SL2Vec3f>[maxY + 1 - minY];

            // füllt die Liste auf (ist performance technisch besser)
            for (int s = 0; s < allXonY.Length; s++)
            {
                allXonY[s] = new List<SL2Vec3f>();
            }

            // fügt alle x auf der gleichen y achse in einer Liste hinzu die der gleiche index hat wie y
            for (int i = 0; i < allPoints.Count; i++)
            {

                allXonY[(int)allPoints[i].y - minY].Add(new SL2Vec3f( allPoints[i], allColors[i]));
            }

            #endregion



            // geht durch jedes y axis hindurch
            for (int indexY = 0; indexY < allXonY.Length; indexY++)
            {
                anzahlX = allXonY[indexY].Count;

                if (anzahlX == 0)
                {
                    continue;
                }

                // sortiert alle x der reihe nach
                allSortedXonY[indexY] = allXonY[indexY].OrderBy(v => v.vectorPosition.x).ToList<SL2Vec3f>();
                maxId                 = anzahlX - 1;

                // setzt start x(1) und end x(2)
                x1                    = (int) allSortedXonY[ indexY ][ 0 ].vectorPosition.x;
                x2                    = (int) allSortedXonY[ indexY ][ maxId ].vectorPosition.x;

                // setzt start farbe und end farbe
                startC.Set( allSortedXonY[ indexY] [ 0 ].vectorColor.x,
                            allSortedXonY[ indexY ][ 0 ].vectorColor.y, 
                            allSortedXonY[ indexY ][ 0 ].vectorColor.z);

                endC.Set( allSortedXonY[ indexY ][ maxId ].vectorColor.x, 
                          allSortedXonY[ indexY ][ maxId ].vectorColor.y, 
                          allSortedXonY[ indexY ][ maxId ].vectorColor.z);


                // zeichnet die line auf der höhe vom index Y
                draw1DLine(x1, x2, indexY + minY,startC,endC);

            }



        }

        /// <summary>
        /// Draws line only on one y axis
        /// </summary>
        /// <param name="x1"> left point</param>
        /// <param name="x2"> right point</param>
        /// <param name="y"> y axis on wich the line will be drawn</param>
        /// <param name="startColor"> color from x1 </param>
        /// <param name="endColor"> color from x2 </param>
        private void draw1DLine(int x1, int x2, int y, SLVec3f startColor, SLVec3f endColor)
        {

            
            this.data          = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            stride             = data.Stride;


            int stepX          = 3, stepY = stride * y, increX = 1;
            int activeX        = x1;

            // Color calculation setup
            SLVec3f newColor   = new SLVec3f();
            //SLVec3f startColor = new SLVec3f();
            //SLVec3f endColor = new SLVec3f();
            double distance;
            double length      = Math.Abs(activeX - x2);



            
            int activePosition = (x1 * stepX) + stepY;
            int endPosition    = x2 * stepX + stepY;


            unsafe
            {
                byte* ptr = (byte*)data.Scan0;

                //startColor.Set((float)ptr[activePosition], (float)ptr[activePosition + 1], (float)ptr[activePosition + 2]);
                //endColor.Set((float)ptr[endPosition], (float)ptr[endPosition + 1], (float)ptr[endPosition + 2]);
               
                while (activePosition != endPosition)
                {

                    distance                = activeX - x1;
                    float ratio             = (float)(distance / length);
                    if (ratio > 1)          { ratio = 1; }
                    newColor                = startColor + ratio * (endColor - startColor);
                    ptr[activePosition]     = (byte)newColor.z;
                    ptr[activePosition + 1] = (byte)newColor.y;
                    ptr[activePosition + 2] = (byte)newColor.x;

                    activePosition          += stepX;
                    activeX                 += increX;
                    // zBuffer[y, activeX] = startZ + ratio * (endZ - startZ);
                }
                ptr[endPosition]            = (byte)endColor.z;
                ptr[endPosition + 1]        = (byte)endColor.y;
                ptr[endPosition + 2]        = (byte)endColor.x;

            }
            bmp.UnlockBits(data);


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
        }

        // inverses the values in the parameters to it negative.
        private void invertValues(ref int slope, ref int mulitpilcator, ref int counter)
        {
            slope         = -slope;
            mulitpilcator = -mulitpilcator;
            counter       = -counter;
        }

        /// <summary>
        /// Gives the Result of all actions performent by BmpG
        /// </summary>
        /// <returns>Bitmap with all the Graphics in it</returns>
        public Bitmap Result()
        {
            return bmp;
        }
    }
}
