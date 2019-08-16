using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ch03_HelloCube_Net
{
    class SL2Vec3f
    {
        public SLVec3f vectorPosition;
        public SLVec3f vectorColor;
        public SL2Vec3f(SLVec3f vPosition, SLVec3f vColor)
        {
            vectorPosition = vPosition;
            vectorColor = vColor;
        }

    }
}
