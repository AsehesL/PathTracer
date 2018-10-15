using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    class TestShader : Shader
    {

        public override Color Render(Tracer tracer, Sky sky, SamplerBase sampler, Ray ray, RayCastHit hit, double epsilon)
        {
            Vector3 refl = Vector3.Reflect(ray.direction, hit.normal);
            if (sky != null)
                return sky.RenderColor(refl);
            return Color.white;
        }

        public override Color FastRender(Ray ray, RayCastHit hit)
        {
            float vdn = (float)Math.Max(0, Vector3.Dot(-1.0 * ray.direction, hit.normal));
            return new Color(vdn, vdn, vdn, 1.0f);
        }
    }
}
