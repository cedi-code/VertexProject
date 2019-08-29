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
        public SLMat4f  m_modelMatrix;
        public Mesh(int vSize, int iSize)
        {
            vertices = new SLVertex[vSize];
            indices = new int[iSize];
            color = new SLVec3f(255, 0, 0);
            m_modelMatrix = new SLMat4f();
        }



        public abstract void build();

    }
}
