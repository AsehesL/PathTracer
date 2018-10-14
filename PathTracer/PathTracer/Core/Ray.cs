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

        public Ray(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
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
        public int depth;
        public Shader shader;
        public double distance;
    }
}
