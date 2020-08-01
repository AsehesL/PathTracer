using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    struct PBRProperty
    {
        public Color albedo;
        public float roughness;
        public float metallic;
        public Color emissive;
        public float refractive;
        public bool tangentSpaceNormal;
        public Color normal;
        public float occlusion;
    }

    abstract class PBRShaderBase : Shader
    {
        protected const float kSunRough = 0.12f;

        protected abstract PBRProperty SampleProperty(Ray ray, RayCastHit hit);

        protected abstract BRDFBase SpecularBRDF { get; }

        protected abstract BRDFBase DiffuseBRDF { get; }

        protected Vector3 RecalucateNormal(Vector3 normal, Vector4 tangent, Color normalColor) 
        {
            Vector3 wbnormal = Vector3.Cross(normal, tangent.xyz).normalized * tangent.w;

            Vector3 rnormal = new Vector3(normalColor.r * 2 - 1, normalColor.g * 2 - 1, normalColor.b * 2 - 1) * -1;
            Vector3 worldnormal = default(Vector3);
            worldnormal.x = tangent.x * rnormal.x + wbnormal.x * rnormal.y + normal.x * rnormal.z;
            worldnormal.y = tangent.y * rnormal.x + wbnormal.y * rnormal.y + normal.y * rnormal.z;
            worldnormal.z = tangent.z * rnormal.x + wbnormal.z * rnormal.y + normal.z * rnormal.z;
            worldnormal.Normalize();
            if (Vector3.Dot(worldnormal, normal) < 0)
                worldnormal *= -1;
            return worldnormal;
        }

        protected Vector3 ImportanceSampleGGX(Vector2 sample, float roughness)
        {
            float a = roughness * roughness;

            double phi = 2.0f * Math.PI * sample.x;
            double cos_theta = Math.Sqrt((1.0 - sample.y) / (1.0 + (a * a - 1.0) * sample.y));
            double sin_theta = Math.Sqrt(1.0 - cos_theta * cos_theta);

            return new Vector3(Math.Cos(phi) * sin_theta, Math.Sin(phi) * sin_theta, cos_theta);
        }

        protected Vector3 ImportanceGGXDirection(Vector3 direction, SamplerBase sampler, float roughness)
        {
            Vector3 w = direction;
            Vector3 u = Vector3.Cross(new Vector3(0.00424f, 1, 0.00764f), w);
            u.Normalize();
            Vector3 v = Vector3.Cross(u, w);
            Vector3 sp = ImportanceSampleGGX(sampler.Sample(), roughness);
            Vector3 l = sp.x * u + sp.y * v + sp.z * w;
            if (Vector3.Dot(l, direction) < 0.0)
                l = -sp.x * u - sp.y * v - sp.z * w;
            l.Normalize();
            return l;
        }

        protected float FresnelSchlickRoughness(float cosTheta, float F0, float roughness)
        {
            return F0 + (Math.Max(1.0f - roughness, F0) - F0) * (float)Math.Pow(1.0f - cosTheta, 5.0f);
        }

        protected Vector3 FresnelSchlickRoughness(float cosTheta, Vector3 F0, float roughness)
        {
            Vector3 f = new Vector3(Math.Max(1.0f - roughness, F0.x), Math.Max(1.0f - roughness, F0.y),
                Math.Max(1.0f - roughness, F0.z));
            return F0 + (f - F0) * (float)Math.Pow(1.0f - cosTheta, 5.0f);
        }

        protected float FresnelSchlickRoughnessRefractive(float cosTheta, float refractive, float roughness)
        {
            float F0 = (1.0f - refractive) / (1.0f + refractive);
            F0 = F0 * F0;
            return F0 + (Math.Max(1.0f - roughness, F0) - F0) * (float)Math.Pow(1.0f - cosTheta, 5.0f);
        }

        //private CookTorranceBRDF m_CookTorranceBRDF = new CookTorranceBRDF();
        //private Color SuperTest(SamplerBase sampler, Ray ray, RayCastHit hit)
        //{
        //    PBRProperty property = SampleProperty(ray, hit);

        //    Vector3 L = ImportanceGGXDirection(hit.normal, sampler, property.roughness);

        //    //float smith = m_CookTorranceBRDF.G_SmithGGX(-1.0 * ray.direction, L, hit.normal, property.roughness);

        //    //float ggx = m_CookTorranceBRDF.GGX_D((L - ray.direction).normalized, hit.normal, property.roughness);

        //    //return new Color(ggx, ggx, ggx, 1.0f);

        //    float ndv = (float)Math.Max(0.0f, Vector3.Dot(hit.normal, -1.0 * ray.direction));
        //    //float F = FresnelSchlickRoughness(ndv, 0.04f, property.roughness);

        //    //if (sampler.GetRandom() < F)
        //    //{
        //    //    float brdf = m_CookTorranceBRDF.BRDF(-1.0 * ray.direction, L, hit, property);

        //    //    return new Color(brdf, brdf, brdf, 1.0f);
        //    //}
        //    //return new Color(0, 0, 0, 1);
        //    //return new Color(F, F, F, 1.0f);

        //    //return new Color(smith, smith, smith, 1.0f);

        //    Vector3 F = FresnelSchlickRoughness(ndv, new Vector3(property.albedo.r, property.albedo.g, property.albedo.b), property.roughness);

        //    return new Color((float)F.x, (float)F.y, (float)F.z, 1.0f);
        //}

        public override Color Render(Tracer tracer, Sky sky, SamplerBase sampler, Ray ray, RayCastHit hit, double epsilon)
        {

            PBRProperty property = SampleProperty(ray, hit);

            Vector3 worldNormal = property.tangentSpaceNormal ? RecalucateNormal(hit.normal, hit.tangent, property.normal) : hit.normal;

            Color result = default(Color);

            if(sky.hasSun)
            {
                result += RenderDirectionalLighting(tracer, sky, sampler, ray, hit.hit, worldNormal, property);
            }

            result += property.occlusion * RenderIndirectLighting(tracer, sky, sampler, ray, hit.hit, worldNormal, property, hit.depth);

            return result + property.emissive;
        }

        private Color RenderDirectionalLighting(Tracer tracer, Sky sky, SamplerBase sampler, Ray ray, Vector3 worldPoint, Vector3 worldNormal, PBRProperty property)
        {
            if (sampler.GetRandom() < property.albedo.a)
            {
                float ndv = (float)Math.Max(0.0f, Vector3.Dot(worldNormal, -1.0 * ray.direction));
                if (sampler.GetRandom() < property.metallic)
                {
                    //Metallic
                    Vector3 F = FresnelSchlickRoughness(ndv, new Vector3(property.albedo.r, property.albedo.g, property.albedo.b), property.roughness);
                    Vector3 L = ImportanceGGXDirection(-1.0 * sky.sunDirection, sampler, kSunRough);
                    double ndl = Vector3.Dot(worldNormal, L);
                    if (ndl < 0.0)
                        return Color.black;
                    Ray lray = new Ray(worldPoint, L);
                    bool shadow = tracer.TracingOnce(lray);
                    if (shadow)
                        return Color.black;
                    ndl = Math.Max(ndl, 0.0);
                    return sky.sunColor * sky.sunIntensity * new Color((float)F.x, (float)F.y, (float)F.z, 1.0f) * SpecularBRDF.BRDFDirectional(-1.0 * ray.direction, L, worldNormal, property) * (float)ndl;
                }
                else
                {
                    //Dielectric
                    float F = FresnelSchlickRoughness(ndv, 0.04f, property.roughness);
                    if (sampler.GetRandom() < F)
                    {
                        //Specular
                        Vector3 L = ImportanceGGXDirection(-1.0 * sky.sunDirection, sampler, kSunRough);
                        double ndl = Vector3.Dot(worldNormal, L);
                        if (ndl < 0.0)
                            return Color.black;
                        Ray lray = new Ray(worldPoint, L);
                        bool shadow = tracer.TracingOnce(lray);
                        if (shadow)
                            return Color.black;
                        ndl = Math.Max(ndl, 0.0);
                        return sky.sunColor * sky.sunIntensity * SpecularBRDF.BRDFDirectional(-1.0 * ray.direction, L, worldNormal, property) * (float)ndl;
                    }
                    else
                    {
                        //Diffuse
                        Vector3 L = ImportanceGGXDirection(-1.0 * sky.sunDirection, sampler, kSunRough);
                        double ndl = Vector3.Dot(worldNormal, L);
                        if (ndl < 0.0)
                            return Color.black;
                        Ray lray = new Ray(worldPoint, L);
                        bool shadow = tracer.TracingOnce(lray);
                        if (shadow)
                            return Color.black;
                        ndl = Math.Max(ndl, 0.0);
                        return property.albedo * sky.sunColor * sky.sunIntensity * DiffuseBRDF.BRDFDirectional(-1.0 * ray.direction, L, worldNormal, property) * (float)ndl;
                    }
                }
            }
            else
            {
                //Transparency
                Vector3 N = ImportanceGGXDirection(worldNormal, sampler, property.roughness);
                float et;
                Vector3 n;
                float cosine;
                float reflectProb;
                float ndv = (float)Vector3.Dot(ray.direction, N);
                if (ndv > 0)
                {
                    n = -1 * N;
                    et = property.refractive;
                    cosine = property.refractive * ndv;

                }
                else
                {
                    n = N;
                    et = 1.0f / property.refractive;
                    cosine = -ndv;
                }

                Vector3 refrac;
                if (Vector3.Refract(ray.direction, n, et, out refrac))
                {
                    reflectProb = FresnelSchlickRoughnessRefractive(cosine, property.refractive, property.roughness);
                }
                else
                {
                    reflectProb = 1.0f;
                }

                if (sampler.GetRandom() < reflectProb)
                {
                    Vector3 L = ImportanceGGXDirection(-1.0 * sky.sunDirection, sampler, kSunRough);
                    double ndl = Vector3.Dot(worldNormal, L);
                    if (ndl < 0.0)
                        return Color.black;
                    Ray lray = new Ray(worldPoint, L);
                    bool shadow = tracer.TracingOnce(lray);
                    if (shadow)
                        return Color.black;
                    ndl = Math.Max(ndl, 0.0);
                    return sky.sunColor * sky.sunIntensity * SpecularBRDF.BRDFDirectional(-1.0 * ray.direction, L, worldNormal, property) * (float)ndl;
                }
                else
                {
                    return Color.black;
                }
            }
        }

        private Color RenderIndirectLighting(Tracer tracer, Sky sky, SamplerBase sampler, Ray ray, Vector3 worldPoint, Vector3 worldNormal, PBRProperty property, int depth)
        {
            if (sampler.GetRandom() < property.albedo.a)
            {
                //Opaque
                float ndv = (float)Math.Max(0.0f, Vector3.Dot(worldNormal, -1.0 * ray.direction));
                if (sampler.GetRandom() < property.metallic)
                {
                    //Metallic
                    Vector3 F = FresnelSchlickRoughness(ndv, new Vector3(property.albedo.r, property.albedo.g, property.albedo.b), property.roughness);
                    Vector3 N = ImportanceGGXDirection(worldNormal, sampler, property.roughness);
                    Vector3 L = Vector3.Reflect(ray.direction * -1, N);
                    Ray lray = new Ray(worldPoint, L);
                    Color col = tracer.Tracing(lray, sky, sampler, depth + 1);
                    float ndl = (float)Math.Max(Vector3.Dot(worldNormal, L), 0.0);
                    return new Color(col.r * (float)F.x, col.g * (float)F.y, col.b * (float)F.z, col.a) * SpecularBRDF.BRDF(-1.0 * ray.direction, L, worldNormal, property) * ndl;
                }
                else
                {
                    //Dielectric
                    float F = FresnelSchlickRoughness(ndv, 0.04f, property.roughness);
                    if (sampler.GetRandom() < F)
                    {
                        //Specular
                        Vector3 N = ImportanceGGXDirection(worldNormal, sampler, property.roughness);
                        Vector3 L = Vector3.Reflect(ray.direction * -1, N);
                        Ray lray = new Ray(worldPoint, L);
                        float ndl = (float)Math.Max(Vector3.Dot(worldNormal, L), 0.0);
                        return tracer.Tracing(lray, sky, sampler, depth + 1) * SpecularBRDF.BRDF(-1.0 * ray.direction, L, worldNormal, property) * ndl;
                    }
                    else
                    {
                        //Diffuse
                        Vector3 w = worldNormal;
                        Vector3 u = Vector3.Cross(new Vector3(0.00424f, 1, 0.00764f), w);
                        u.Normalize();
                        Vector3 v = Vector3.Cross(u, w);
                        Vector3 sp = sampler.SampleHemiSphere(0);

                        Vector3 wi = sp.x * u + sp.y * v + sp.z * w;
                        wi.Normalize();

                        float ndl = (float)Math.Max(0.0f, Vector3.Dot(worldNormal, wi));

                        Ray lray = new Ray(worldPoint, wi);
                        return ndl * property.albedo * tracer.Tracing(lray, sky, sampler, depth + 1) * DiffuseBRDF.BRDF(-1.0 * ray.direction, wi, worldNormal, property);
                    }
                }
            }
            else
            {
                //Transparency
                Vector3 N = ImportanceGGXDirection(worldNormal, sampler, property.roughness);
                float et;
                Vector3 n;
                float cosine;
                float reflectProb;
                float ndv = (float)Vector3.Dot(ray.direction, N);
                if (ndv > 0)
                {
                    n = -1 * N;
                    et = property.refractive;
                    cosine = property.refractive * ndv;

                }
                else
                {
                    n = N;
                    et = 1.0f / property.refractive;
                    cosine = -ndv;
                }

                Vector3 refrac;
                if (Vector3.Refract(ray.direction, n, et, out refrac))
                {
                    reflectProb = FresnelSchlickRoughnessRefractive(cosine, property.refractive, property.roughness);
                }
                else
                {
                    reflectProb = 1.0f;
                }

                if (sampler.GetRandom() < reflectProb)
                {
                    Ray lray = new Ray(worldPoint, Vector3.Reflect(ray.direction * -1, N));
                    return tracer.Tracing(lray, sky, sampler, depth + 1);
                }
                else
                {
                    Ray lray = new Ray(worldPoint, refrac);
                    return property.albedo * tracer.Tracing(lray, sky, sampler, depth + 1);
                }
            }
        }

        public override Color FastRender(Ray ray, RayCastHit hit)
        {
            //float vdn = (float)Math.Max(0, Vector3.Dot(-1.0 * ray.direction, hit.normal));

            PBRProperty property = SampleProperty(ray, hit);

            //Vector3 worldNormal = property.tangentSpaceNormal ? RecalucateNormal(hit.normal, hit.tangent, property.normal) : hit.normal;

            return property.albedo;
            //return Color.white * property.occlusion;

            //return new Color((float)worldNormal.x * 0.5f + 0.5f, (float)worldNormal.y * 0.5f + 0.5f, (float)worldNormal.z * 0.5f + 0.5f, 1.0f);
            //return new Color(vdn, vdn, vdn, 1.0f);
        }
    }
}
