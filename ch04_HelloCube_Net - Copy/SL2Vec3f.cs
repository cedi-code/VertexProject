using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ch03_HelloCube_Net
{
    class SL2Vec3f
    {
        public SLVec3f vectorPosition;
        public SLVec3f vectorPolygonFriend1;
        public SLVec3f vectorPolygonFriend2;
        public SLVec3f vectorColor;
        private SLVec3f normale0;
        private SLVec3f normale1;
        private SLVec3f normale2;

        public SL2Vec3f(SLVec3f vPosition, SLVec3f vColor)
        {
            vectorPosition = vPosition;
            vectorColor = vColor;
            normale0 = new SLVec3f(1, 0, 0);
            normale1 = new SLVec3f(0, 1, 0);
            normale2 = new SLVec3f(0, 0, 1);
        }

        public SLVec3f getNormale0()
        {

            return normale0;
        }
        public SLVec3f getNormale1()
        {
            return normale1;
        }
        public SLVec3f getNormale2()
        {
            return normale2;
        }




    }
}
