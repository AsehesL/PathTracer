using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public class Quad : Plane
    {
        public double widthSquared;
        public double heightSquared;
        public Vector3 right;
        public Vector3 up;

        public Quad(Vector3 position, Vector3 normal, Vector3 right, Vector3 up, Shader shader) : base(position, normal, shader)
        {
            this.widthSquared = right.sqrMagnitude;
            this.heightSquared = up.sqrMagnitude;
            this.right = right;
            this.up = up;
        }

        public override bool RayCast(Ray ray, double epsilon, ref RayCastHit hit)
        {
            double t = Vector3.Dot(this.position - ray.origin, this.normal) / Vector3.Dot(ray.direction, this.normal);
            if (t < epsilon)
                return false;
            if (t > hit.distance)
                return false;
            Vector3 p = ray.origin + ray.direction * t;
            Vector3 d = p - this.position;
            double ddw = Vector3.Dot(d, this.right);
            if (ddw < 0.0 || ddw > this.widthSquared)
                return false;
            double ddh = Vector3.Dot(d, this.up);
            if (ddh < 0.0 || ddh > this.heightSquared)
                return false;
            hit.distance = t;
            hit.normal = normal;
            hit.texcoord = Vector2.zero;
            hit.normal = this.normal;
            hit.shader = shader;
            hit.hit = p;
            return true;
        }
    }
}
