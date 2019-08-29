using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ch03_HelloCube_Net
{
    class Sphere : Mesh
    {
        private float radius;
        private int stacks;
        private int slices;
        public Sphere(float radius, int stacks, int slices) 
              : base((stacks + 1) * (slices + 1), (slices * stacks * 2 * 3))
        {
            this.radius = radius;
            this.stacks = stacks;
            this.slices = slices;

            build();
        }

        public override void build()
        {
            float theta, dtheta; // angles around x-axis
            float phi, dphi;     // angles around z-axis
            int i, j;          // loop counters
            int iv = 0;


            // init start values
            theta = 0.0f;
            dtheta = (float)Math.PI / stacks;
            dphi = 2.0f * (float)Math.PI / slices;

            // Define vertex position & normals by looping through all stacks
            for (i = 0; i <= stacks; ++i)
            {
                float sin_theta = (float)Math.Sin(theta);
                float cos_theta = (float)Math.Cos(theta);
                phi = 0.0f;

                // Loop through all slices
                for (j = 0; j <= slices; ++j)
                {
                    if (j == slices) { phi = 0.0f; }

                    // is unnecessary
                    vertices[iv] = new SLVertex();
                    vertices[iv].setColor(color);
                    // define first the normal with length 1
                    vertices[iv].normale.x = sin_theta * (float)Math.Cos(phi);
                    vertices[iv].normale.y = sin_theta * (float)Math.Sin(phi);
                    vertices[iv].normale.z = cos_theta;

                    // set the vertex position w. the scaled normal
                    vertices[iv].position.x = radius * vertices[iv].normale.x;
                    vertices[iv].position.y = radius * vertices[iv].normale.y;
                    vertices[iv].position.z = radius * vertices[iv].normale.z;

                    // set the texture coords.
                    //vertices[iv].t.x = 0; // ???
                    //vertices[iv].t.y = 0; // ???

                    phi += dphi;
                    iv++;
                }
                theta += dtheta;
            }

            // create Index array x
            // neighbors
            int ii = 0, iV1, iV2;
            for (i = 0; i < stacks; ++i)
            {
                // index of 1st & 2nd vertex of stack
                iV1 = i * (slices + 1);
                iV2 = iV1 + slices + 1;

                for (j = 0; j < slices; ++j)
                { // 1st triangle ccw
                    indices[ii++] = iV1 + j;
                    indices[ii++] = iV2 + j;
                    indices[ii++] = iV2 + j + 1;
                    // 2nd triangle ccw
                    indices[ii++] = iV1 + j;
                    indices[ii++] = iV2 + j + 1;
                    indices[ii++] = iV1 + j + 1;
                }
            }
        }
    }
}
