using System;
using System.Collections.Generic;

namespace ASL.PathTracer
{
    [ShaderType("Transparency")]
    class TransparencyShader : Shader
    {
        public Color color = Color.white;
        public float refractive;
        public float roughness;

        public override Color Render(Tracer tracer, Sky sky, SamplerBase sampler, Ray ray, RayCastHit hit, double epsilon)
        {
            Vector3 snormal;
            if (roughness > 10000)
            {
                Vector3 sp = sampler.SampleHemiSphere(roughness);

                Vector3 u = Vector3.Cross(new Vector3(0.00424f, 1, 0.00764f), hit.normal);
                u.Normalize();
                Vector3 v = Vector3.Cross(u, hit.normal);
                snormal = sp.x * u + sp.y * v + sp.z * hit.normal;
                if (Vector3.Dot(snormal, hit.normal) < 0.0)
                    snormal = -sp.x * u - sp.y * v - sp.z *hit.normal;
                snormal.Normalize();
            }
            else
            {
                snormal = hit.normal;
            }

            float et;
            Vector3 n;
            double cosine;
            double vdn = Vector3.Dot(ray.direction, snormal);
            double reflectProb;
			if (vdn > 0)
            {
                n = -1 * snormal;
                et = refractive;
                cosine = refractive * vdn;

            }
            else
            {
                n = snormal;
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
	            Vector3 w = Vector3.Reflect(ray.direction * -1, snormal);
				Ray lray = new Ray(hit.hit, w);
	            return tracer.Tracing(lray, sky, sampler, hit.depth + 1);
			}
            else
            {
	            Ray lray = new Ray(hit.hit, refrac);
	            return color*tracer.Tracing(lray, sky, sampler, hit.depth + 1);
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

        Vector3 ImportanceSampleGGX(Vector2 sample, float roughness)
        {
            float a = roughness * roughness;

            double phi = 2.0f * Math.PI * sample.x;
            double cos_theta = Math.Sqrt((1.0 - sample.y) / (1.0 + (a * a - 1.0) * sample.y));
            double sin_theta = Math.Sqrt(1.0 - cos_theta * cos_theta);

            return new Vector3(Math.Cos(phi) * sin_theta, Math.Sin(phi) * sin_theta, cos_theta);
        }
    }
}
