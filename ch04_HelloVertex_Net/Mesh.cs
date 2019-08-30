using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ch03_HelloCube_Net
{
    abstract class Mesh
    {
        public SLVertex[]  vertices;
        public int[]       indices;
        public SLVec3f     color;
        public SLMat4f  modelMatrix;
        public Mesh(int vSize, int iSize)
        {
            vertices = new SLVertex[vSize];
            indices = new int[iSize];
            color = new SLVec3f();
            modelMatrix = new SLMat4f();
        }

        public Mesh(int vSize, int iSize, SLVec3f color)
        {
            vertices = new SLVertex[vSize];
            indices = new int[iSize];
            this.color = color;
            modelMatrix = new SLMat4f();
        }




        public abstract void build();

    }
}
