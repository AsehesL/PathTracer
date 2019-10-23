using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public class Sphere : BoundsGeometry
	{
        public Vector3 position { get; set; }
        public double radius { get; set; }

        public Sphere(Vector3 position, double radius, Shader shader) : base(shader)
        {
            this.position = position;
            this.radius = radius;

            this.bounds = new Bounds(position, Vector3.one * (radius * 2.0));
        }

		public override bool RayCast(Ray ray, double epsilon, ref RayCastHit hit)
        {
            Vector3 tocenter = ray.origin - this.position;

            double vala = Vector3.Dot(ray.direction, ray.direction);
            double valb = Vector3.Dot(tocenter, ray.direction) * 2.0f;
            double valc = Vector3.Dot(tocenter, tocenter) - radius * radius;

            double dis = valb * valb - 4.0f * vala * valc;


            if (dis < 0.0)
                return false;
            else
            {
                double e = Math.Sqrt(dis);
                double denom = 2.0f * vala;
                double t = (-valb - e) / denom;

                if (t > epsilon && t <= hit.distance)
                {
                    // if (hit.depth == 0 && m_Material.isLight && LScene.ignoreLight)
                    //     return false;
                    hit.distance = t;
                    hit.normal = (tocenter + ray.direction * t) / radius;
                    hit.hit = ray.origin + ray.direction * t;
                    hit.shader = shader;
                    hit.texcoord = default(Vector2);
                    return true;
                }


                t = (-valb + e) / denom;

                if (t > epsilon && t <= hit.distance)
                {
                    // if (hit.depth == 0 && m_Material.isLight && LScene.ignoreLight)
                    //     return false;
                    hit.distance = t;
                    hit.normal = (tocenter + ray.direction * t) / radius;
                    hit.hit = ray.origin + ray.direction * t;
                    hit.shader = shader;
                    hit.texcoord = default(Vector2);
                    return true;
                }
            }

            return false;
        }
    }
}
