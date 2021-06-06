using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public class Sphere : Geometry
    {
	    public Vector3 position;
        public double radius;

        private float m_InvArea;

        public Sphere(Vector3 position, double radius, Material material) : base(material)
        {
            this.position = position;
            this.radius = radius;

            m_InvArea = 1.0f / ((float)(4.0 * Math.PI * radius * radius));

            this.m_bounds = new Bounds(position, Vector3.one * (radius * 2.0));
        }

		protected override bool RayCastGeometry(Ray ray, ref RayCastHit hit)
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

                if (t > double.Epsilon && t <= hit.distance)
                {

                    hit.distance = t;
                    hit.normal = (tocenter + ray.direction * hit.distance) / radius;
                    hit.hit = ray.origin + ray.direction * hit.distance;
                    hit.material = material;
                    hit.geometry = this;
                    hit.texcoord = GetUV(hit.normal);
                    Vector3 tang = Vector3.Cross(new Vector3(0, 1, 0), hit.normal).normalized;
                    hit.tangent = new Vector4(tang.x, tang.y, tang.z, 1.0);

                    double ndv = Vector3.Dot(hit.normal, ray.origin - hit.hit);
                    if (ndv < 0)
                    {
                        if (material != null && !material.ShouldRenderBackFace())
                            return false;
                        hit.isBackFace = true;
                        //hit.normal *= -1;
                    }
                    else
                        hit.isBackFace = false;

                    if (hit.isBackFace)
                        hit.hit -= hit.normal * 0.00000000000001;
                    else
                        hit.hit += hit.normal * 0.00000000000001;
                    //hit.distance = Vector3.Distance(hit.hit, ray.origin);

                    return true;
                }


                t = (-valb + e) / denom;

                if (t > double.Epsilon && t <= hit.distance)
                {

                    hit.distance = t;
                    hit.normal = (tocenter + ray.direction * hit.distance) / radius;
                    hit.hit = ray.origin + ray.direction * hit.distance;
                    hit.material = material;
                    hit.geometry = this;
                    hit.texcoord = GetUV(hit.normal);
                    Vector3 tang = Vector3.Cross(new Vector3(0, 1, 0), hit.normal).normalized;
                    hit.tangent = new Vector4(tang.x, tang.y, tang.z, 1.0);

                    double ndv = Vector3.Dot(hit.normal, ray.origin - hit.hit);
                    if (ndv < 0)
                    {
                        if (material != null && !material.ShouldRenderBackFace())
                            return false;
                        hit.isBackFace = true;
                        //hit.normal *= -1;
                    }
                    else
                        hit.isBackFace = false;

                    if (hit.isBackFace)
                        hit.hit -= hit.normal * 0.00000000000001;
                    else
                        hit.hit += hit.normal * 0.00000000000001;
                    //hit.distance = Vector3.Distance(hit.hit, ray.origin);

                    return true;
                }
            }

            return false;
        }

        public override Vector3 GetNormal(Vector3 point)
        {
            return (point - position).normalized;
        }

        public override Vector3 Sample(SamplerBase sampler)
        {
            return position + sampler.SampleSphere() * radius;
        }

        public override float GetPDF()
        {
            return m_InvArea;
        }

        private Vector2 GetUV(Vector3 normal)
        {
            double theta = Math.Acos(-normal.y);
            double phi = Math.Atan2(-normal.z, normal.x) + Math.PI;

            double u = phi / (2 * Math.PI);
            double v = theta / Math.PI;

            return new Vector2(u, v);
        }
    }
}
