using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ch03_HelloCube_Net
{
    /// <summary>
    /// light saves all values needed to calculate light
    /// </summary>
    class SLLight
    {
        public SLVec3f diffuse;
        public SLVec3f ambient;
        public SLVec3f direction;
        public SLVec3f mirror;
        public bool isPhong;

        /// <summary>
        /// create new light with 
        /// diffuse 1 1 1
        /// ambient 0,1 0,1 0,1
        /// direction 0 0 1
        /// mirror 0,8 0,8 0,8
        /// </summary>
        public SLLight()
        {
            this.diffuse = new SLVec3f(1, 1, 1);
            this.ambient = new SLVec3f(0.1f, 0.1f, 0.1f);
            this.direction = new SLVec3f(0, 0, 1);
            this.mirror = new SLVec3f(0.8f, 0.8f, 0.8f);
            isPhong = false;
        }
    }
}
