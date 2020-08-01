using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    [ShaderType("CamTest")]
    class CamTestShader : Shader
    {
        public Color color;

        public override Color Render(Tracer tracer, Sky sky, SamplerBase sampler, Ray ray, RayCastHit hit, double epsilon)
        {
            float vdn = (float)Math.Max(0, Vector3.Dot(-1.0 * ray.direction, hit.normal));
            Color difcol = color;

            difcol *= vdn;
            return difcol;
        }

        public override Color FastRender(Ray ray, RayCastHit hit)
        {
            float vdn = (float)Math.Max(0, Vector3.Dot(-1.0 * ray.direction, hit.normal));
            Color difcol = color;

            difcol *= vdn;
            return difcol;
        }
    }
}