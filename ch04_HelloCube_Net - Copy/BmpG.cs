using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows;
using System.Linq;
using System.Diagnostics;

namespace ch03_HelloCube_Net
{
    class BmpG : IDisposable
    {
        #region values
        private Bitmap bmp;
        private BitmapData data;
        private int stride;
        private ZBuffer zB;
        private SLLight light;
        public bool phong = false;
        public bool xRay = false;
        #endregion

        /// <summary>
        /// Creates a Bitmap with set height and width
        /// </summary>
        /// <param name="width">Width of the bitmap</param>
        /// <param name="height">Height of the bitmap</param>
        public BmpG(int width, int height, ZBuffer buffer)
        {
            this.bmp = new Bitmap(width, height);
            zB = buffer;
            this.light = null;
            this.phong = false;
            this.data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            stride = data.Stride;

        }
        public BmpG(int width, int height, ZBuffer buffer, SLLight light)
        {
            this.bmp = new Bitmap(width, height);
            zB = buffer;
            this.light = light;
            this.phong = false;
            this.data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            stride = data.Stride;

        }

        /// <summary>
        /// Draws Line in the Bitmap
        /// </summary>
        /// <param name="start">starting vector</param>
        /// <param name="end">ending vector</param>
        /// <param name="start_color">starting color of starting vector</param>
        /// <param name="end_color">ending color of ending vector</param>
        public void DrawLine(SLVertex start, SLVertex end)
        {
            this.data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            stride = data.Stride;
            List<SLVertex> noPointsNeeded      = new List<SLVertex>();
            Clipping_Line(start, end, ref noPointsNeeded);

            bmp.UnlockBits(data);
        }

        /// <summary>
        /// Draws Line in the Bitmap and sets up the Primitives
        /// </summary>
        /// <param name="startVec">starting vector</param>
        /// <param name="endVec">ending vector</param>
        /// <param name="start_color">starting color of starting vector</param>
        /// <param name="end_color">ending color of ending vector</param>
        /// <param name="points">saves the coordinates of the line</param>
        /// <param name="colors">saves the colors of the line</param>
        private void DrawLine(SLVertex start, SLVertex end, ref List<SLVertex> primitves)
        {


            #region setup


            // color calculations
            double distance = 0;
            SLVec3f newColor = new SLVec3f();

            SLVec3f startC = start.color, endC = end.color;
            SLVec3f startVec = start.position, endVec = end.position;


            RoundVector(ref startVec);
            RoundVector(ref endVec);




            // calculates slopes
            int dx = (int)(endVec.x - startVec.x);
            int dy = (int)(endVec.y - startVec.y);

            // steps size for calculation in ptr array
            int stepX = 3, stepY = stride;
            int stepDX = 1, stepDY = 1;

            // positions to know the coordinates of the active position
            int posY = (int)startVec.y;
            int posX = (int)startVec.x;
            float posZ;
            float startZ = startVec.z;
            float endZ = endVec.z;

            // transformed starting coordinates for the ptr array
            int activeY = (int)startVec.y * stride;
            int activeX = (int)startVec.x * stepX;

            // transformed ending coordinates for the ptr array
            int lastX = (int)endVec.x * stepX;
            int lastY = (int)endVec.y * stepY;

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
            double lenght = Math.Sqrt(Math.Pow(endVec.x - startVec.x, 2) + Math.Pow(endVec.y - startVec.y, 2));


            #endregion
            if (phong)
            {

                SLVec3f newViewSpaceP = new SLVec3f();
                SLVec3f startVSP = start.posInView, endVSP = end.posInView;

                SLVec3f startNorm = start.normale, endNorm = end.normale;
                SLVec3f newNormale = new SLVec3f();

                unsafe
                {
                    // gets the first pixel adress in the bitmap
                    byte* ptr = (byte*)data.Scan0;

                    if (dx > dy)
                    {
                        int dE = (dy << 1); // dy*2 anstatt dx  / 2
                        int dNE = (dy << 1) - (dx << 1); // dy*2 - dx*2
                        int D = (dy << 1) - dx; // dy*2 - dx

                        while (activeX != lastX)
                        {



                            #region calculation
                            //                                             ___________________________
                            // distance between start and active pixel    √ (x1 - x0)^2 + (y1 - y0)^2
                            distance = Math.Sqrt((Math.Pow(posX - startVec.x, 2) + Math.Pow(posY - startVec.y, 2)));
                            // distance percent between the whole line
                            float ratio = (float)(distance / lenght);
                            if (ratio > 1) { ratio = 1; }
                            // gradient between start color and end color
                            // nur berechnen wenn zBuffer stimmt???
                            posZ = startZ + ratio * (endZ - startZ);
                            newNormale = startNorm + ratio * (endNorm - startNorm);
                            newColor = startC + ratio * (endC - startC);
                            newViewSpaceP = startVSP + ratio * (endVSP - startVSP);
                            SLVertex nVertex = new SLVertex(posX, posY, posZ, newNormale, newColor, newViewSpaceP);
                            primitves.Add(nVertex);
                            #endregion

                            if (zB.checkZ(posX, posY, posZ))
                            {
                                newColor = nVertex.colorToLight(light);
                                // set active pixel the rgb (r=x, g=y, b=z)
                                ptr[activeX + activeY] = (byte)(newColor.z);
                                ptr[activeX + activeY + 1] = (byte)(newColor.y);
                                ptr[activeX + activeY + 2] = (byte)(newColor.x);
                            }



                            // saves points to create the primitives





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

                            #region color calculation
                            //                                             ___________________________
                            // distance between start and active pixel    √ (x1 - x0)^2 + (y1 - y0)^2
                            distance = Math.Sqrt((Math.Pow(posX - startVec.x, 2) + Math.Pow(posY - startVec.y, 2)));
                            float ratio = (float)(distance / lenght);
                            if (ratio > 1) { ratio = 1; }
                            newColor = startC + ratio * (endC - startC);
                            posZ = startZ + ratio * (endZ - startZ);
                            newNormale = startNorm + ratio * (endNorm - startNorm);
                            newViewSpaceP = startVSP + ratio * (endVSP - startVSP);
                            SLVertex nVertex = new SLVertex(posX, posY, posZ, newNormale, newColor, newViewSpaceP);
                            primitves.Add(nVertex);
                            #endregion

                            if (zB.checkZ(posX, posY, posZ))
                            {
                                newColor = nVertex.colorToLight(light);
                                // set active pixel the rgb (r=x, g=y, b=z)
                                ptr[activeX + activeY] = (byte)(newColor.z);
                                ptr[activeX + activeY + 1] = (byte)(newColor.y);
                                ptr[activeX + activeY + 2] = (byte)(newColor.x);
                            }

                            // saves points to create the primitives

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
                                posX += stepDX;

                            }

                        }
                    }
                    // set last pixel 
                    if (zB.checkZ(posX, posY, endVec.z))
                    {
                        endC = end.colorToLight(light);
                        ptr[lastX + lastY] = (byte)(endC.z);
                        ptr[lastX + lastY + 1] = (byte)(endC.y);
                        ptr[lastX + lastY + 2] = (byte)(endC.x);
                    }

                    primitves.Add(new SLVertex(endVec, endNorm, endC, endVSP));

                }
            }
            // ohne phong
            else
            {
                unsafe
                {
                    // gets the first pixel adress in the bitmap
                    byte* ptr = (byte*)data.Scan0;

                    if (dx > dy)
                    {
                        int dE = (dy << 1); // dy*2 anstatt dx  / 2
                        int dNE = (dy << 1) - (dx << 1); // dy*2 - dx*2
                        int D = (dy << 1) - dx; // dy*2 - dx

                        while (activeX != lastX)
                        {



                            #region calculation
                            //                                             ___________________________
                            // distance between start and active pixel    √ (x1 - x0)^2 + (y1 - y0)^2
                            distance = Math.Sqrt((Math.Pow(posX - startVec.x, 2) + Math.Pow(posY - startVec.y, 2)));
                            // distance percent between the whole line
                            float ratio = (float)(distance / lenght);
                            if (ratio > 1) { ratio = 1; }
                            // gradient between start color and end color
                            // nur berechnen wenn zBuffer stimmt???
                            posZ = startZ + ratio * (endZ - startZ);
                            newColor = startC + ratio * (endC - startC);
                            primitves.Add(new SLVertex(posX, posY, posZ, newColor));
                            #endregion

                            if (zB.checkZ(posX, posY, posZ))
                            {
                                // set active pixel the rgb (r=x, g=y, b=z)
                                ptr[activeX + activeY] = (byte)(newColor.z);
                                ptr[activeX + activeY + 1] = (byte)(newColor.y);
                                ptr[activeX + activeY + 2] = (byte)(newColor.x);
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

                            #region color calculation
                            //                                             ___________________________
                            // distance between start and active pixel    √ (x1 - x0)^2 + (y1 - y0)^2
                            distance = Math.Sqrt((Math.Pow(posX - startVec.x, 2) + Math.Pow(posY - startVec.y, 2)));
                            float ratio = (float)(distance / lenght);
                            if (ratio > 1) { ratio = 1; }
                            newColor = startC + ratio * (endC - startC);
                            posZ = startZ + ratio * (endZ - startZ);
                            primitves.Add(new SLVertex(posX, posY, posZ, newColor));
                            #endregion

                            if (zB.checkZ(posX, posY, posZ))
                            {
                                // set active pixel the rgb (r=x, g=y, b=z)
                                ptr[activeX + activeY] = (byte)(newColor.z);
                                ptr[activeX + activeY + 1] = (byte)(newColor.y);
                                ptr[activeX + activeY + 2] = (byte)(newColor.x);
                            }

                            // saves points to create the primitives

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
                                posX += stepDX;

                            }

                        }
                    }
                    // set last pixel 
                    if (zB.checkZ(posX, posY, endVec.z))
                    {
                        endC = end.colorToLight(light);
                        ptr[lastX + lastY] = (byte)(endC.z);
                        ptr[lastX + lastY + 1] = (byte)(endC.y);
                        ptr[lastX + lastY + 2] = (byte)(endC.x);
                    }

                    primitves.Add(new SLVertex(endVec, end.normale, endC));

                }
            }
        }


       

        /// <summary>
        /// Draws an fill in a Polygon counterclockwise
        /// </summary>
        /// <param name="vec0">first vector</param>
        /// <param name="c0">color from vector 0 </param>
        /// <param name="vec1">second vector</param>
        /// <param name="c1">color from vector 1 </param>
        /// <param name="vec2">third and last vector</param>
        /// <param name="c2">color4 from vector 2 </param>
        public void DrawPolygon(SLVertex vertex0, SLVertex vertex1, SLVertex vertex2)
        {
            //Clipping_Line(v0,v1);
            //Clipping_Line(v1, v2);
            //Clipping_Line(v2, v0);

            // check if vectors are at the boarder;
            // Draws the line between the 3 vectors and saves the primitves

            SLVertex v0 = new SLVertex(), v1 = new SLVertex(), v2 = new SLVertex();
            v0.Set(vertex0);v1.Set(vertex1);v2.Set(vertex2);



            List<SLVertex> allPrimitves = new List<SLVertex>();


            SLVertex randV0 = Clipping_Line(v0,v1, ref allPrimitves);
            SLVertex randV1 = Clipping_Line(v1,v2,ref allPrimitves);
            SLVertex randV2 = Clipping_Line(v2,v0, ref allPrimitves);

            // todo draw rand line
            if(randV0 != null && randV1 !=null)
            {
                DrawLine(randV0, randV1, ref allPrimitves);

            }else if(randV1 != null && randV2 != null)
            {
                DrawLine(randV1, randV2, ref allPrimitves);
            }
            else if (randV2 != null && randV0 != null)
            {
                DrawLine(randV2, randV0, ref allPrimitves);
   
            }
            //if (!phong)
            //{
            //    v0.color = v0.colorToLight(light); v1.color = v1.colorToLight(light); v2.color = v2.colorToLight(light);
            //}




            if (allPrimitves.Count < 2 || xRay)
            {
                return;
            }



            SLVec3f vec0 = v0.position, vec1 = v1.position, vec2 = v2.position;
            SLVec3f c0 = v0.color, c1 = v1.color, c2 = v2.color;
            #region values
            int minY, maxY;




            float[] yPoints = { vec0.y, vec1.y, vec2.y };

            int anzahlX = 0;
            SLVertex x1, x2;
            int maxId = 0;
            SLVec3f startC = new SLVec3f();
            SLVec3f endC = new SLVec3f();

            #endregion

            #region calculate setup

            // calculates the square around the triangle
            minY            = (int)yPoints.Min();
            maxY            = (int)Math.Round(yPoints.Max(),0);





            // creates list on the size of the square (boundries)
            List<SLVertex>[] allXonY       = new List<SLVertex>[maxY + 1 - minY];
            List<SLVertex>[] allSortedXonY = new List<SLVertex>[maxY + 1 - minY];

            // füllt die Liste auf (ist performance technisch besser)
            for (int s = 0; s < allXonY.Length; s++)
            {
                allXonY[s] = new List<SLVertex>();
            }

            // fügt alle x auf der gleichen y achse in einer Liste hinzu die der gleiche index hat wie y
            for (int i = 0; i < allPrimitves.Count; i++)
            {

                allXonY[(int)allPrimitves[i].position.y - minY].Add(allPrimitves[i]);
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

                // TODO änder das es im vertex sucht!
                // sortiert alle x der reihe nach
                allSortedXonY[indexY] = allXonY[indexY].OrderBy(v => v.position.x).ToList<SLVertex>();
                maxId                 = anzahlX - 1;

                // setzt start x(1) und end x(2)
                x1                    = allSortedXonY[ indexY ][ 0 ];
                
                x2                    = allSortedXonY[ indexY ][ maxId ];

                if(x1.position.x == x2.position.x)
                {
                    continue;
                }
                draw1DLine(x1, x2, indexY + minY);
                
  
                // zeichnet die line auf der höhe vom index Y


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
        private void draw1DLine(SLVertex x1, SLVertex x2, int y)
        {




            int stepX          = 3, stepY = stride * y, increX = 1;
            int activeX        = (int)x1.position.x;
            int startX         = activeX;
            float startZ       = x1.position.z;
            float activeZ      = startZ;
            float endZ         = x2.position.z;

            // Color calculation setup
            SLVec3f newColor   = new SLVec3f();
            //SLVec3f startColor = new SLVec3f();
            //SLVec3f endColor = new SLVec3f();
            double distance;
            double length      = Math.Abs(activeX - x2.position.x);

            float zDiffrence = endZ - startZ;
            SLVec3f colorDiffrence = x2.color - x1.color;

            SLVec3f startColor     = x1.color;
            SLVec3f endColor       = x2.color;

            SLVec3f nNormale = new SLVec3f();
            SLVertex nVertex = new SLVertex();

            int activePosition = (int)(startX * stepX) + stepY;
            int endPosition    = (int)x2.position.x * stepX + stepY;

            if (phong)
            {
                SLVec3f startNormale = x1.normale, endNormale = x2.normale;
                SLVec3f normaleDiffrence = endNormale - startNormale;
                nNormale = startNormale;

                SLVec3f startVSP = x1.posInView, endVSP = x2.posInView;
                SLVec3f posVSDiffrence = endVSP - startVSP;
                SLVec3f nViewSpaceP = startVSP;

                unsafe
                {
                    byte* ptr = (byte*)data.Scan0;

                    //startColor.Set((float)ptr[activePosition], (float)ptr[activePosition + 1], (float)ptr[activePosition + 2]);
                    //endColor.Set((float)ptr[endPosition], (float)ptr[endPosition + 1], (float)ptr[endPosition + 2]);

                    while (activePosition != endPosition)
                    {

                        distance = activeX - startX;
                        float ratio = (float)(distance / length); // todo bytes ändern anstatt durch zu rechnen?
                        activeZ = startZ + ratio * zDiffrence;
                        if (zB.checkZ(activeX, y, activeZ))
                        {
                            // TODO intorpolate normale and calc color
                            newColor = startColor + ratio * colorDiffrence;
                            nNormale = startNormale + ratio * normaleDiffrence;
                            nViewSpaceP = startVSP + ratio * posVSDiffrence;

                            nVertex = new SLVertex(activeX, y, activeZ, nNormale, newColor, nViewSpaceP);
                            newColor = nVertex.colorToLight(light);
                            ptr[activePosition] = (byte)newColor.z;
                            ptr[activePosition + 1] = (byte)newColor.y;
                            ptr[activePosition + 2] = (byte)newColor.x;
                        }


                        activePosition += stepX;
                        activeX += increX;
                        // zBuffer[y, activeX] = startZ + ratio * (endZ - startZ);
                    }
                    if (zB.checkZ((int)x2.position.x, y, x2.position.z))
                    {
                        endColor = x2.colorToLight(light);
                        ptr[endPosition] = (byte)endColor.z;
                        ptr[endPosition + 1] = (byte)endColor.y;
                        ptr[endPosition + 2] = (byte)endColor.x;
                    }


                }
            }
            else
            {
                unsafe
                {
                    byte* ptr = (byte*)data.Scan0;

                    //startColor.Set((float)ptr[activePosition], (float)ptr[activePosition + 1], (float)ptr[activePosition + 2]);
                    //endColor.Set((float)ptr[endPosition], (float)ptr[endPosition + 1], (float)ptr[endPosition + 2]);

                    while (activePosition != endPosition)
                    {

                        distance = activeX - startX;
                        float ratio = (float)(distance / length); // todo bytes ändern anstatt durch zu rechnen?
                        activeZ = startZ + ratio * zDiffrence;
                        if (zB.checkZ(activeX, y, activeZ))
                        {
                            // TODO intorpolate normale and calc color
                            newColor = startColor + ratio * colorDiffrence;
                            ptr[activePosition] = (byte)newColor.z;
                            ptr[activePosition + 1] = (byte)newColor.y;
                            ptr[activePosition + 2] = (byte)newColor.x;
                        }


                        activePosition += stepX;
                        activeX += increX;
                        // zBuffer[y, activeX] = startZ + ratio * (endZ - startZ);
                    }
                    if (zB.checkZ((int)x2.position.x, y, x2.position.z))
                    {
                        ptr[endPosition] = (byte)endColor.z;
                        ptr[endPosition + 1] = (byte)endColor.y;
                        ptr[endPosition + 2] = (byte)endColor.x;
                    }


                }
            }

   


        }


        
        private SLVertex Clipping_Line(SLVertex vertex0,SLVertex vertex1, ref List<SLVertex> allPrimitves)
        {
            SLVertex v0 = new SLVertex();
            v0.Set(vertex0);
            SLVertex v1 = new SLVertex();
            v1.Set(vertex1);
            SLVertex border = null;

            bool f = false;
            while (!f)
            {
                int codeA = set4Bit(v0.position.x, v0.position.y);
                int codeB = set4Bit(v1.position.x, v1.position.y);
               
                if ((codeA | codeB) == 0)
                {
                    f = true;

                }
                if((codeA & codeB) != 0)
                {
                    return null;
                }

                if (codeA > 0)
                {
                    calcPoints(ref v0,ref  v1, codeA);
                    border = v0;
                }
                else if (codeB > 0)
                {
                    calcPoints(ref v1, ref v0, codeB);
                    border = v1;
                }



            }
            if (!phong)
            {
                v0.color = v0.colorToLight(light); v1.color = v1.colorToLight(light);
            }
            DrawLine(v0, v1, ref allPrimitves);

            if(border != null)
            {
                return border;
            }
            
            return null;


        }

        int left = 1, right = 2, top = 4, bottom = 8;
        private void calcPoints(ref SLVertex v1, ref SLVertex v2, int code)
        {
            float x1 = v1.position.x, y1 = v1.position.y;
            float x2 = v2.position.x, y2 = v2.position.y;
            if((code & top) > 0)
            {
                v1.position.x = x1 + (x2 - x1) * (0 - y1) / (y2 - y1);
                v1.position.y = 0;
                // achtung nicht verwechseln!
            }else if((code & bottom) > 0)
            {
                v1.position.x = x1 + (x2 - x1) * (bmp.Height-1 - y1) / (y2 - y1);
                v1.position.y = bmp.Height-1;
            }
            else if((code & right) > 0)
            {
                v1.position.y = y1 + (y2 - y1) * (bmp.Width-1 - x1) / (x2 - x1);
                v1.position.x = bmp.Width-1;
            }
            else if((code & left) > 0 )
            {
                v1.position.y = y1 + (y2 - y1) * (0- x1) / (x2 - x1);
                v1.position.x = 0;
            }
            else
            {
                v1.position.y = -1;
                v1.position.x = -1;
            }




        }



        private int set4Bit(float x, float y)
        {

            int bitcode = 0;
            if(x < 0)
            {
                bitcode = bitcode | left;
            }
            else if(x > bmp.Width-1)
            {
                bitcode = bitcode | right;
            }
            if(y < 0)
            {
                bitcode = bitcode | top;
            }
            else if(y > bmp.Height -1)
            {
                bitcode = bitcode | bottom;
            }
            return bitcode;
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
            bmp.UnlockBits(data);

            return bmp;
            
        }

        public void Dispose()
        {
            bmp.Dispose();
        }
    }
}
