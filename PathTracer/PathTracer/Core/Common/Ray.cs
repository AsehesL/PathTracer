using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public struct Ray
    {
        public Vector3 origin;

        public Vector3 direction;

        public bool isDiffuseRay;

        public Ray(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
            this.isDiffuseRay = false;
        }

        public Ray(Vector3 origin, Vector3 direction, bool diffuseRay)
        {
            this.origin = origin;
            this.direction = direction;
            this.isDiffuseRay = diffuseRay;
        }

        public override string ToString()
        {
            return $"Origin:{origin},Direction:{direction}";
        }
    }

    public struct RayCastHit
    {

        public Vector3 hit;
        public Vector2 texcoord;
        public Vector3 normal;
		public Vector4 tangent;
        public int depth;
        public Material material;
        public Geometry geometry;
        public bool isBackFace;
        public double distance;
    }
}
