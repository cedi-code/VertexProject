﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ch03_HelloCube_Net
{
    /// <summary>
    /// saves all values in a Vertex
    /// (can also calculate light)
    /// </summary>
    class SLVertex
    {
        public SLVec3f position;
        public SLVec3f posInView;
        public SLVec3f normale;
        public SLVec3f color;
        public SLVec3f nColor;

        /// <summary>
        /// creates new Vertex with all new SLVec3();
        /// nColor is not set!
        /// </summary>
        public SLVertex()
        {
            position = new SLVec3f();
            normale = new SLVec3f();
            color = new SLVec3f();
            posInView = new SLVec3f();
        }
        public SLVertex(SLVec3f pos, SLVec3f normale, SLVec3f color)
        {
            this.position = pos;
            this.normale = normale;
            this.color = color;
            this.nColor = color / 255;
            this.posInView = null;
        }

        public SLVertex(SLVec3f pos, SLVec3f normale, SLVec3f color, SLVec3f pIV)
        {
            this.position = pos;
            this.normale = normale;
            this.color = color;
            this.nColor = color / 255;
            this.posInView = pIV;
        }
        public SLVertex(float px, float py, float pz, SLVec3f col)
        {
            this.position = new SLVec3f(px,py,pz);
            this.normale = null;
            this.color = col;
            this.nColor = col / 255;
        }
        public SLVertex(float px, float py, float pz, SLVec3f norm, SLVec3f col, SLVec3f pVI)
        {
            this.position = new SLVec3f(px, py, pz);
            this.normale = norm;
            this.color = col;
            this.nColor = color / 255;
            this.posInView = pVI;
        }

        /// <summary>
        /// calculates a new color relative to the light
        /// </summary>
        /// <param name="light">relative to this light</param>
        /// <returns>new color</returns>
        public SLVec3f colorToLight(SLLight light)
        {
            if(light.isPhong)
            {
                this.normale.Normalize();
            }
            SLVec3f c = spiegel(light) + difuse(light);
            return checkColor(c) *255;
        }

        /// <summary>
        /// diffuses the color relative to its light distance
        /// </summary>
        /// <param name="light"></param>
        /// <returns></returns>
        public SLVec3f difuse(SLLight light)
        {
            float NdL = Math.Max(SLVec3f.DotProduct(this.normale, light.direction), 0);
            SLVec3f colorD = ((nColor & light.diffuse) * NdL);
            return colorD;
        }

        /// <summary>
        /// calculates mirror effect from normale relative to the light
        /// </summary>
        /// <param name="light"></param>
        /// <returns></returns>
        public SLVec3f spiegel(SLLight light)
        {
            SLVec3f R =  2 * (SLVec3f.DotProduct(light.direction, this.normale)) * this.normale - light.direction;
            SLVec3f E = -(this.posInView);
            E.Normalize();
            float RsE = (float) Math.Pow(Math.Max(SLVec3f.DotProduct(R, E),0),5);
            return (light.mirror) * RsE;
        }

        /// <summary>
        /// shorts the values to 1, prevents out of range
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private SLVec3f checkColor(SLVec3f c)
        {
            return new SLVec3f(Math.Min(c.x, 1), Math.Min(c.y, 1), Math.Min(c.z, 1));
        }

        /// <summary>
        /// sets color of this vertex
        /// </summary>
        /// <param name="c">color</param>
        public void setColor(SLVec3f c)
        {
            this.color = c;
            this.nColor = c / 255;
        }

        /// <summary>
        /// sets but dosn't create a vertex
        /// </summary>
        /// <param name="nVertex">new Vertex values</param>
        public void Set(SLVertex nVertex)
        {
            this.position.Set(nVertex.position);
            this.normale.Set(nVertex.normale);
            this.color.Set(nVertex.color);
            this.nColor = color / 255;
            this.posInView.Set(nVertex.posInView);
        }


        

    }
}
