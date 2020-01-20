using System;
using System.Collections.Generic;

namespace ASL.PathTracer
{
    [ShaderType("Transparency")]
    class TransparencyShader : Shader
    {
        public float refractive;

        public override Color Render(Tracer tracer, Sky sky, SamplerBase sampler, Ray ray, RayCastHit hit, double epsilon)
        {
            //Vector3 w = Vector3.Reflect(ray.direction.normalized, hit.normal);
            float et;
            Vector3 n;
            if (Vector3.Dot(ray.direction, hit.normal) > 0)
            {
                n = -1 * hit.normal;
                et = refractive;
            }
            else
            {
                n = hit.normal;
                et = 1.0f / refractive;
            }

            Vector3 refrac;
            if (Vector3.Refract(ray.direction, n, et, out refrac))
            {
                Ray lray = new Ray(hit.hit, refrac);
                return tracer.Tracing(lray, sky, sampler, hit.depth + 1);
            }
            else
            {
                return Color.black;
            }
        }

        public override Color FastRender(Ray ray, RayCastHit hit)
        {
            return Color.black;
        }
    }
}
