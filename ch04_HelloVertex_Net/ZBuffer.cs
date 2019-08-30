using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ch03_HelloCube_Net
{
    /// <summary>
    /// saves all Z positions of each pixel
    /// </summary>
    class ZBuffer
    {
        private float[,] buffer;
        private int height, width;
        public float near, far;

        /// <summary>
        /// sets up a new ZBuffer
        /// </summary>
        /// <param name="width">with of all pixels</param>
        /// <param name="height">height of all pixels</param>
        /// <param name="n">nearest value</param>
        /// <param name="f">farest value</param>
        public ZBuffer(int width, int height, float n, float f)
        {
            buffer = new float[width, height];
            this.height = height;
            this.width = width;
            this.near = n;
            this.far = f;
            Reset();
        }

        /// <summary>
        /// resets all pixels z value to far
        /// </summary>
        public void Reset()
        {
            for(int w = 0; w < width; w++)
            {
                for(int h = 0; h < height; h++)
                {
                    buffer[w, h] = far;
                }
            }
        }
        /// <summary>
        /// checks if pixel is between near and far
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="z">z position/value</param>
        /// <returns></returns>
        public bool checkZ(int x, int y, float z)
        {
            if(buffer[x,y] > z && z > near)
            {
                buffer[x, y] = z;
                return true;
            }
            return false;
        }
    }
}
