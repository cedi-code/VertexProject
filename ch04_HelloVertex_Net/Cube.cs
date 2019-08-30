using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ch03_HelloCube_Net
{
    /// <summary>
    /// creates a 3D Cube out of a Mesh
    /// </summary>
    class Cube : Mesh
    {
        private float size = 1.0f;
        private SLVec3f[] vectors;
        private SLVec3f front, back, left, right, top, bottom;

        /// <summary>
        /// creates new Cube 
        /// 24 vertices
        /// 36 indices
        /// </summary>
        /// <param name="size">length of the lines</param>
        public Cube(float size) : base(24,36)
        {
            vectors = new SLVec3f[8];
            vectors[0] = new SLVec3f(-1f, -1f,  1f); // front lower left
            vectors[1] = new SLVec3f( 1f, -1f,  1f); // front lower right
            vectors[2] = new SLVec3f( 1f,  1f,  1f); // front upper right
            vectors[3] = new SLVec3f(-1f,  1f,  1f); // front upper left
            vectors[4] = new SLVec3f(-1f, -1f, -1f); // back lower left
            vectors[5] = new SLVec3f( 1f, -1f, -1f); // back lower right
            vectors[6] = new SLVec3f( 1f,  1f, -1f); // back upper left
            vectors[7] = new SLVec3f(-1f,  1f, -1f); // back upper right

            front = new SLVec3f(0, 0, 1);
            back = new SLVec3f(0, 0, -1);
            left = new SLVec3f(-1, 0, 0);
            right = new SLVec3f(1, 0, 0);
            top = new SLVec3f(0, 1, 0);
            bottom = new SLVec3f(0, -1, 0);

            modelMatrix.Scale(size,size,size);

            build();


        }

        public override void build()
        {
            #region setUpVertices
            int ii = 0;
            vertices[ii++] = new SLVertex(vectors[0], front, color);
            vertices[ii++] = new SLVertex(vectors[1], front, color);
            vertices[ii++] = new SLVertex(vectors[2], front, color);
            vertices[ii++] = new SLVertex(vectors[3], front, color);
            vertices[ii++] = new SLVertex(vectors[3], left, color);
            vertices[ii++] = new SLVertex(vectors[7], left, color);
            vertices[ii++] = new SLVertex(vectors[4], left, color);
            vertices[ii++] = new SLVertex(vectors[0], left, color);
            vertices[ii++] = new SLVertex(vectors[7], back, color);
            vertices[ii++] = new SLVertex(vectors[6], back, color);
            vertices[ii++] = new SLVertex(vectors[5], back, color);
            vertices[ii++] = new SLVertex(vectors[4], back, color);
            vertices[ii++] = new SLVertex(vectors[1], right, color);
            vertices[ii++] = new SLVertex(vectors[5], right, color);
            vertices[ii++] = new SLVertex(vectors[6], right, color);
            vertices[ii++] = new SLVertex(vectors[2], right, color);
            vertices[ii++] = new SLVertex(vectors[2], top, color);
            vertices[ii++] = new SLVertex(vectors[6], top, color);
            vertices[ii++] = new SLVertex(vectors[7], top, color);
            vertices[ii++] = new SLVertex(vectors[3], top, color);
            vertices[ii++] = new SLVertex(vectors[0], bottom, color);
            vertices[ii++] = new SLVertex(vectors[4], bottom, color);
            vertices[ii++] = new SLVertex(vectors[5], bottom, color);
            vertices[ii++] = new SLVertex(vectors[1], bottom, color);
            #endregion

            #region setUpIndices
            int start = 0;
            int up = 1;
            for (int i = 0; i < 36; i += 6)
            {
                for (int s = 0; s < 4; s += 3)
                {
                    for (int d = 1; d < 3; d++)
                    {
                        indices[i + s + d] = up;
                        up++;
                    }
                    up--;
                    indices[i + s] = start;
                }
                start += 4;
                up = start + 1;
            }
            #endregion
        }

        /// <summary>
        /// change size of cube
        /// </summary>
        /// <param name="s">new size</param>
        public void setSize(float s)
        {
            this.size = s;
            modelMatrix.Scale(size, size, size);
        }
    }
}
