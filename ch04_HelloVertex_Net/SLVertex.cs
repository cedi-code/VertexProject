using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ch03_HelloCube_Net
{
    class SLVertex
    {
        public SLVec3f position;
        public SLVec3f posInView;
        public SLVec3f normale;
        public SLVec3f color;
        public SLVec3f nColor;


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


        public SLVec3f colorToLight(SLLight light)
        {
            // TODO nur Normalizierte farben speichern, damit man nur 1mal  * 255 rechnen muss!
            // nur bei phong normalisieren :)
            if(light.isPhong)
            {
                this.normale.Normalize();
            }

            return checkColor(spiegel(light) + difuse(light)) *255;
        }

        public SLVec3f difuse(SLLight light)
        {
            float NdL = Math.Max(SLVec3f.DotProduct(this.normale, light.direction), 0);
            SLVec3f colorD = ((nColor & light.diffuse) * NdL);
            return colorD;
        }
        public SLVec3f spiegel(SLLight light)
        {
            SLVec3f R =  2 * (SLVec3f.DotProduct(light.direction, this.normale)) * this.normale - light.direction;
            SLVec3f E = -(this.posInView);
            E.Normalize();
            float RsE = (float) Math.Pow(Math.Max(SLVec3f.DotProduct(R, E),0),5);
            return (light.mirror) * RsE;
        }

        private SLVec3f checkColor(SLVec3f c)
        {
            return new SLVec3f(Math.Min(c.x, 1), Math.Min(c.z, 1), Math.Min(c.z, 1));
        }

        public void setColor(SLVec3f c)
        {
            this.color = c;
            this.nColor = c / 255;
        }

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
