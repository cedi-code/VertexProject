using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ch03_HelloCube_Net
{
    class Cube : Mesh
    {
        private float size = 1.0f;
        private SLVec3f[] m_v;
        private SLVec3f front, back, left, right, top, bottom;
        public Cube(float size) : base(24,36)
        {
            m_v = new SLVec3f[8];
            m_v[0] = new SLVec3f(-1f, -1f,  1f); // front lower left
            m_v[1] = new SLVec3f( 1f, -1f,  1f); // front lower right
            m_v[2] = new SLVec3f( 1f,  1f,  1f); // front upper right
            m_v[3] = new SLVec3f(-1f,  1f,  1f); // front upper left
            m_v[4] = new SLVec3f(-1f, -1f, -1f); // back lower left
            m_v[5] = new SLVec3f( 1f, -1f, -1f); // back lower right
            m_v[6] = new SLVec3f( 1f,  1f, -1f); // back upper left
            m_v[7] = new SLVec3f(-1f,  1f, -1f); // back upper right

            front = new SLVec3f(0, 0, 1);
            back = new SLVec3f(0, 0, -1);
            left = new SLVec3f(-1, 0, 0);
            right = new SLVec3f(1, 0, 0);
            top = new SLVec3f(0, 1, 0);
            bottom = new SLVec3f(0, -1, 0);

            m_modelMatrix.Scale(size,size,size);

            build();


        }

        public override void build()
        {
            int ii = 0;
            vertices[ii++] = new SLVertex(m_v[0], front, color);
            vertices[ii++] = new SLVertex(m_v[1], front, color);
            vertices[ii++] = new SLVertex(m_v[2], front, color);
            vertices[ii++] = new SLVertex(m_v[3], front, color);
            vertices[ii++] = new SLVertex(m_v[3], left, color);
            vertices[ii++] = new SLVertex(m_v[7], left, color);
            vertices[ii++] = new SLVertex(m_v[4], left, color);
            vertices[ii++] = new SLVertex(m_v[0], left, color);
            vertices[ii++] = new SLVertex(m_v[7], back, color);
            vertices[ii++] = new SLVertex(m_v[6], back, color);
            vertices[ii++] = new SLVertex(m_v[5], back, color);
            vertices[ii++] = new SLVertex(m_v[4], back, color);
            vertices[ii++] = new SLVertex(m_v[1], right, color);
            vertices[ii++] = new SLVertex(m_v[5], right, color);
            vertices[ii++] = new SLVertex(m_v[6], right, color);
            vertices[ii++] = new SLVertex(m_v[2], right, color);
            vertices[ii++] = new SLVertex(m_v[2], top, color);
            vertices[ii++] = new SLVertex(m_v[6], top, color);
            vertices[ii++] = new SLVertex(m_v[7], top, color);
            vertices[ii++] = new SLVertex(m_v[3], top, color);
            vertices[ii++] = new SLVertex(m_v[0], bottom, color);
            vertices[ii++] = new SLVertex(m_v[4], bottom, color);
            vertices[ii++] = new SLVertex(m_v[5], bottom, color);
            vertices[ii++] = new SLVertex(m_v[1], bottom, color);


            int start = 0;
            int second = 0;
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
        }

        public void setSize(float s)
        {
            this.size = s;
            m_modelMatrix.Scale(size, size, size);
        }
    }
}
