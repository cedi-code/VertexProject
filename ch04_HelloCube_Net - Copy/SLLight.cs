using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ch03_HelloCube_Net
{

    class SLLight
    {
        public SLVec3f diffuse;
        public SLVec3f ambient;
        public SLVec3f direction;
        public SLVec3f mirror;
        public bool isPhong;
        public SLLight()
        {
            this.diffuse = new SLVec3f(1, 1, 1);
            this.ambient = new SLVec3f(0.1f, 0.1f, 0.1f);
            this.direction = new SLVec3f(0, 0, 1);
            this.mirror = new SLVec3f(0.8f, 0.8f, 0.8f);
            isPhong = false;
        }
        public SLLight(SLVec3f diffuse, SLVec3f direction)
        {
            this.direction = direction;
            this.diffuse = diffuse;
        }
    }
}
