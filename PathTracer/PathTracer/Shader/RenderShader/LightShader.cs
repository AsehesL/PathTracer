using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{


    class LightShader : Shader
    {
        public Color color;
        public float indentity;

        public override Color Render(Tracer tracer, Sky sky, SamplerBase sampler, Ray ray, RayCastHit hit, double epsilon)
        {
            //float ndl = (float)Math.Max(0, Vector3.Dot(-1*ray.direction, hit.normal));

            return color*indentity;
        }
    }
}