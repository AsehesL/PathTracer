using System;
using System.Collections.Generic;
using System.Windows.Forms.Layout;

namespace ASL.PathTracer
{
    [ShaderType("PBR")]
    class PBRShader : Shader
    {
        public Color color;
        public Texture albedo;
		public Texture metallicTex;
		public float metallic;
		public Texture roughnessTex;
		public float roughness;
		public float refractive;
		public Vector2 tile;

        public override Color Render(Tracer tracer, Sky sky, SamplerBase sampler, Ray ray, RayCastHit hit, double epsilon)
        {
			//bool isMetal = metallic > 0.5f;
			float metal = metallic;
	        if (metallicTex != null)
	        {
				//isMetal = isMetal || metallicTex.Sample((float) (hit.texcoord.x * tile.x), (float) (hit.texcoord.y * tile.y)).r > 0.5;
				metal *= metallicTex.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y)).r;
			}

	        float rough = roughness;
	        if (roughnessTex != null)
	        {
		        rough *= roughnessTex.Sample((float) (hit.texcoord.x * tile.x), (float) (hit.texcoord.y * tile.y)).r;
	        }

			Color difcol = color;
			if (albedo != null)
			{
				difcol *= albedo.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y));
			}

			if(sampler.GetRandom() < difcol.a)
			{
				//Opaque
				float ndv = (float)Math.Max(0.0f, Vector3.Dot(hit.normal, -1.0 * ray.direction));

				if (sampler.GetRandom() < metal)
				{
					Vector3 F = FresnelSchlickRoughness(ndv, new Vector3(difcol.r, difcol.g, difcol.b), rough);

					Vector3 w = Vector3.Reflect(ray.direction * -1, hit.normal);
					Vector3 u = Vector3.Cross(new Vector3(0.00424f, 1, 0.00764f), w);
					u.Normalize();
					Vector3 v = Vector3.Cross(u, w);
					Vector3 sp = ImportanceSampleGGX(sampler.Sample(), rough);
					Vector3 l = sp.x * u + sp.y * v + sp.z * w;
					if (Vector3.Dot(l, hit.normal) < 0.0)
						l = -sp.x * u - sp.y * v - sp.z * w;
					l.Normalize();
					Ray lray = new Ray(hit.hit, l);
					Color lcol = tracer.Tracing(lray, sky, sampler, hit.depth + 1);

					float ndl = (float)Math.Max(Vector3.Dot(hit.normal, l), 0.0);

					return new Color(lcol.r * (float)F.x, lcol.g * (float)F.y, lcol.b * (float)F.z, lcol.a) * ndl;
				}
				else
				{
					float F = FresnelSchlickRoughness(ndv, 0.04f, rough);
					if (sampler.GetRandom() < F)
					{
						//Specular
						Vector3 w = Vector3.Reflect(ray.direction * -1, hit.normal);
						Vector3 u = Vector3.Cross(new Vector3(0.00424f, 1, 0.00764f), w);
						u.Normalize();
						Vector3 v = Vector3.Cross(u, w);
						Vector3 sp = ImportanceSampleGGX(sampler.Sample(), rough);
						Vector3 l = sp.x * u + sp.y * v + sp.z * w;
						if (Vector3.Dot(l, hit.normal) < 0.0)
							l = -sp.x * u - sp.y * v - sp.z * w;
						l.Normalize();
						Ray lray = new Ray(hit.hit, l);
						Color lcol = tracer.Tracing(lray, sky, sampler, hit.depth + 1);

						float ndl = (float)Math.Max(Vector3.Dot(hit.normal, l), 0.0);

						return lcol * ndl;
					}
					else
					{
						//Diffuse
						Vector3 w = hit.normal;
						Vector3 u = Vector3.Cross(new Vector3(0.00424f, 1, 0.00764f), w);
						u.Normalize();
						Vector3 v = Vector3.Cross(u, w);
						Vector3 sp = sampler.SampleHemiSphere(0);

						Vector3 wi = sp.x * u + sp.y * v + sp.z * w;
						wi.Normalize();

						float ndl = (float)Math.Max(0.0f, Vector3.Dot(hit.normal, wi));

						Ray lray = new Ray(hit.hit, wi);
						return ndl * difcol * tracer.Tracing(lray, sky, sampler, hit.depth + 1) * Lambertatian() * (1.0f - F);
					}
				}
			}
			else
			{
				//Transparency
				double ndv = Vector3.Dot(hit.normal, -1.0 * ray.direction);

				Vector3 w = hit.normal;
				Vector3 u = Vector3.Cross(new Vector3(0.00424f, 1, 0.00764f), w);
				u.Normalize();
				Vector3 v = Vector3.Cross(u, w);
				Vector3 sp = ImportanceSampleGGX(sampler.Sample(), rough);
				Vector3 l = sp.x * u + sp.y * v + sp.z * w;
				if (Vector3.Dot(l, hit.normal) < 0.0)
					l = -sp.x * u - sp.y * v - sp.z * w;
				l.Normalize();

				float et;
				Vector3 n;
				double cosine;
				double reflectProb;
				if (ndv > 0)
				{
					n = -1 * l;
					et = refractive;
					cosine = refractive * ndv;

				}
				else
				{
					n = l;
					et = 1.0f / refractive;
					cosine = -ndv;
				}

				Vector3 refrac;
				if (Vector3.Refract(ray.direction, n, et, out refrac))
				{
					reflectProb = FresnelSchlickRoughnessRefractive(cosine, refractive, rough);
				}
				else
				{
					reflectProb = 1.0;
				}

				if (sampler.GetRandom() < reflectProb)
				{
					Ray lray = new Ray(hit.hit, Vector3.Reflect(ray.direction * -1, l));
					return tracer.Tracing(lray, sky, sampler, hit.depth + 1);
				}
				else
				{
					Ray lray = new Ray(hit.hit, refrac);
					return difcol * tracer.Tracing(lray, sky, sampler, hit.depth + 1);
				}
			}
		}

        public override Color FastRender(Ray ray, RayCastHit hit)
        {
			float vdn = (float)Math.Max(0, Vector3.Dot(-1.0 * ray.direction, hit.normal));
			Color difcol = color;
			if (albedo != null)
			{
				difcol *= albedo.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y));
			}

			difcol *= vdn;
			return difcol;
		}

        private float Lambertatian()
        {
	        return (float) MathUtils.InvPi;
        }

        float FresnelSchlickRoughness(float cosTheta, float F0, float roughness)
        {
	        return F0 + (Math.Max(1.0f - roughness, F0) - F0) * (float)Math.Pow(1.0f - cosTheta, 5.0f);
		}

		double FresnelSchlickRoughnessRefractive(double cosTheta, float refractive, float roughness)
		{
			double F0 = (1.0 - refractive) / (1.0 + refractive);
			F0 = F0 * F0;
			return F0 + (Math.Max(1.0f - roughness, F0) - F0) * (float)Math.Pow(1.0f - cosTheta, 5.0f);
		}

		Vector3 FresnelSchlickRoughness(float cosTheta, Vector3 F0, float roughness)
        {
	        Vector3 f = new Vector3(Math.Max(1.0f - roughness, F0.x), Math.Max(1.0f - roughness, F0.y),
		        Math.Max(1.0f - roughness, F0.z));
	        return F0 + (f - F0) * (float)Math.Pow(1.0f - cosTheta, 5.0f);
        }

        //float GeometrySchlickGGX(float NdotV, float roughness)
        //{
	       // float r = (roughness + 1.0f);
	       // float k = (r * r) / 8.0f;

	       // float nom = NdotV;
	       // float denom = NdotV * (1.0f - k) + k;

	       // return nom / denom;
        //}
        //float GeometrySmith(float NdotV, float NdotL, float roughness)
        //{
	       // float ggx2 = GeometrySchlickGGX(NdotV, roughness);
	       // float ggx1 = GeometrySchlickGGX(NdotL, roughness);

	       // return ggx1 * ggx2;
        //}

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
