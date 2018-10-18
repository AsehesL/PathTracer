using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    class SpecularShader : Shader 
    {
        public Color color;

        public float e;

        public override Color Render(Tracer tracer, Sky sky, SamplerBase sampler, Ray ray, RayCastHit hit, double epsilon)
        {
            Vector3 w = Vector3.Reflect(ray.direction, hit.normal);
            Vector3 u = Vector3.Cross(new Vector3(0.00424f, 1, 0.00764f), w);
            u.Normalize();
            Vector3 v = Vector3.Cross(u, w);
            Vector3 sp = sampler.SampleHemiSphere(e);

            Vector3 wi = sp.x * u + sp.y * v + sp.z * w;
            if (Vector3.Dot(wi, hit.normal) < 0.0)
                wi = -sp.x * u - sp.y * v - sp.z * w;
            wi.Normalize();

            float ndl = (float)Vector3.Dot(hit.normal, wi);

            Ray lray = new Ray(hit.hit, wi);
            Color realCol = ndl * color * tracer.Tracing(lray, sky, sampler, hit.depth + 1);

            return realCol;
        }
    }
}
