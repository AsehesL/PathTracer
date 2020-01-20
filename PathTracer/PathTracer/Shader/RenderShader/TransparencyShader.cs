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
            Vector3 w = Vector3.Reflect(ray.direction * -1, hit.normal);
            float et;
            Vector3 n;
            double cosine;
            double vdn = Vector3.Dot(ray.direction, hit.normal);
            double reflectProb;
			if (vdn > 0)
            {
                n = -1 * hit.normal;
                et = refractive;
                cosine = refractive * vdn;

            }
            else
            {
                n = hit.normal;
                et = 1.0f / refractive;
				cosine = -vdn;
            }

            Vector3 refrac;
            if (Vector3.Refract(ray.direction, n, et, out refrac))
            {
                //Ray lray = new Ray(hit.hit, refrac);
                //return tracer.Tracing(lray, sky, sampler, hit.depth + 1);
                reflectProb = Schlick(cosine, refractive);
            }
            else
            {
	            reflectProb = 1.0;
	            //return Color.black;
            }

            if (sampler.GetRandom() < reflectProb)
            {
	            Ray lray = new Ray(hit.hit, w);
	            return tracer.Tracing(lray, sky, sampler, hit.depth + 1);
			}
            else
            {
	            Ray lray = new Ray(hit.hit, refrac);
	            return tracer.Tracing(lray, sky, sampler, hit.depth + 1);
			}
        }

        public override Color FastRender(Ray ray, RayCastHit hit)
        {
            return Color.black;
        }

        private double Schlick(double cosine, float r)
        {
	        double r0 = (1.0 - r) / (1.0 + r);
			r0 = r0 * r0;
			return r0 + (1.0 - r0) * Math.Pow(1 - cosine, 5);
        }
    }
}
