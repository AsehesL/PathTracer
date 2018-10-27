using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{


    class LightShader : Shader
    {
        public Color baseColor;
        public Color color;
        public float pow;
        public float indensity;

        public override Color Render(Tracer tracer, Sky sky, SamplerBase sampler, Ray ray, RayCastHit hit, double epsilon)
        {
            float ndv = (float)Math.Max(0, Vector3.Dot(-1*ray.direction, hit.normal));

            Color col = baseColor + color * (float)Math.Pow(ndv, pow) * indensity;

            return col;
        }

        public override Color FastRender(Ray ray, RayCastHit hit)
        {
            float ndv = (float)Math.Max(0, Vector3.Dot(-1 * ray.direction, hit.normal));

            Color col = baseColor + color * (float)Math.Pow(ndv, pow) * indensity;

            return col;
        }
    }
}