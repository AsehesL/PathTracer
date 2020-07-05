using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public class Plane : Geometry
    {
        public Vector3 position;
        public Vector3 normal;

        public Plane(Vector3 position, Vector3 normal, Shader shader) : base(shader)
        {
            this.position = position;
            this.normal = normal;
        }

        protected override bool RayCastGeometry(Ray ray, double epsilon, ref RayCastHit hit)
        {
            double t = Vector3.Dot(this.position - ray.origin, this.normal) / Vector3.Dot(ray.direction, this.normal);
            if (t >= epsilon)
            {
                if (t > hit.distance)
                    return false;
                hit.distance = t;
                hit.hit = ray.origin + ray.direction * t;
                hit.texcoord = Vector2.zero;
                hit.normal = this.normal;
                hit.shader = shader;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
