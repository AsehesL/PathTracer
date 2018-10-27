using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    class DiffuseShader : Shader
    {
        public Color color;
        public Texture diffuse;
        public Vector2 tile;

        public override Color Render(Tracer tracer, Sky sky, SamplerBase sampler, Ray ray, RayCastHit hit, double epsilon)
        {
            Vector3 w = hit.normal;
            Vector3 u = Vector3.Cross(new Vector3(0.00424f, 1, 0.00764f), w);
            u.Normalize();
            Vector3 v = Vector3.Cross(u, w);
            Vector3 sp = sampler.SampleHemiSphere(0);

            Vector3 wi = sp.x * u + sp.y * v + sp.z * w;
            wi.Normalize();

            float ndl = (float)Vector3.Dot(hit.normal, wi);

            Color difcol = color;
            if (diffuse != null)
            {
                difcol *= diffuse.Sample((float) (hit.texcoord.x * tile.x), (float) (hit.texcoord.y * tile.y));
            }

            Ray lray = new Ray(hit.hit, wi);
            Color realCol = ndl * difcol * tracer.Tracing(lray, sky, sampler, hit.depth + 1);

            return realCol;
        }

        public override Color FastRender(Ray ray, RayCastHit hit)
        {
            float vdn = (float)Math.Max(0, Vector3.Dot(-1.0 * ray.direction, hit.normal));
            Color difcol = color;
            if (diffuse != null)
            {
                difcol *= diffuse.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y));
            }

            difcol *= vdn;
            return difcol;
        }
    }
}
