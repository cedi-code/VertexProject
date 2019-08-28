using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ch03_HelloCube_Net
{
    class ZBuffer
    {
        private float[,] buffer;
        private int height, width;
        public float near, far;

        public ZBuffer(int width, int height, float n, float f)
        {
            buffer = new float[width, height];
            this.height = height;
            this.width = width;
            this.near = n;
            this.far = f;
            Reset();
        }

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
        public bool checkZ(int x, int y, float z)
        {
            if(buffer[x,y] > z)
            {
                if(z > near)
                {
                    buffer[x, y] = z;
                    return true;
                }

            }
            return false;
        }
    }
}
