using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ch03_HelloCube_Net
{
    /// <summary>
    /// Mesh of an 3D object
    /// </summary>
    abstract class Mesh
    {
        public SLVertex[]   vertices;
        public int[]        indices;
        public SLVec3f      color;
        public SLMat4f      modelMatrix;

        /// <summary>
        /// number of vertices and indices
        /// </summary>
        /// <param name="vSize">vertices</param>
        /// <param name="iSize">indices</param>
        public Mesh(int vSize, int iSize)
        {
            vertices =      new SLVertex[vSize];
            indices =       new int[iSize];
            color =         new SLVec3f();
            modelMatrix =   new SLMat4f();
        }

        /// <summary>
        /// sets all the vertices and indices
        /// </summary>
        public abstract void build();

    }
}
