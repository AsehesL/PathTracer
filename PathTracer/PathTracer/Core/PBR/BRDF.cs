using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    abstract class BRDF
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract float SampleBRDF(SamplerBase sampler, Vector3 wo, Vector3 wi, Vector3 position, Vector3 normal, float roughness);
    }

    class LambertatianBRDF : BRDF
    {

        public override float SampleBRDF(SamplerBase sampler, Vector3 wo, Vector3 wi, Vector3 position, Vector3 normal, float roughness)
        {
            return (float)(1.0f / Math.PI);
        }
    }

    //class OrenNayarBRDF : BRDF
    //{
    //    public override float SampleBRDF(SamplerBase sampler, Vector3 wo, Vector3 position, Vector3 normal, float roughness, out Vector3 wi, out float pdf)
    //    {
    //        // equivalent to dot(normalize(i.x, i.y, 0), normalize(o.x, o.y, 0)).
    //        // i.e. remove z-component (normal) and get the cos angle between vectors with dot
    //        double cos_delta_phi = glm::clamp((i.x * o.x + i.y * o.y) / std::sqrt((pow2(i.x) + pow2(i.y)) * (pow2(o.x) + pow2(o.y))), 0.0, 1.0);

    //        // D = sin(alpha) * tan(beta), i.z = dot(i, (0,0,1))
    //        double D = std::sqrt((1.0 - pow2(i.z)) * (1.0 - pow2(o.z))) / std::max(i.z, o.z);

    //        // A and B are pre-computed in constructor.
    //        return (reflectance * C::INV_PI) * (A + B * cos_delta_phi * D);
    //    }
    //}

    class DisneyDiffuseBRDF : BRDF
    {
        public override float SampleBRDF(SamplerBase sampler, Vector3 wo, Vector3 wi, Vector3 position, Vector3 normal, float roughness)
        {
            double oneMinusCosL = 1.0 - Vector3.Dot(wi, normal);
            double oneMinusCosLSqr = oneMinusCosL * oneMinusCosL;
            double oneMinusCosV = 1.0 - Vector3.Dot(wo, normal);
            double oneMinusCosVSqr = oneMinusCosV * oneMinusCosV;

            // Roughness是粗糙度，IDotH的意思会在下一篇讲Microfacet模型时提到
            double IDotH = Vector3.Dot(wi, (wi + wo).normalized);
            double F_D90 = 0.5 + 2.0 * IDotH * IDotH * roughness;

            return (float)((1.0 / Math.PI) * (1.0 + (F_D90 - 1.0) * oneMinusCosLSqr * oneMinusCosLSqr * oneMinusCosL) *
                (1.0 + (F_D90 - 1.0) * oneMinusCosVSqr * oneMinusCosVSqr * oneMinusCosV));
        }
    }

    class CookTorranceBRDF : BRDF
    {

        public override float SampleBRDF(SamplerBase sampler, Vector3 wo, Vector3 wi, Vector3 position, Vector3 normal, float roughness)
        {
            double denominator = 4.0 * Math.Max(Vector3.Dot(normal, wi), 0.0) * Math.Max(Vector3.Dot(normal, wo), 0.0) + 0.001;
            //float nominator = D_GGX((wi + wo).normalized, normal, roughness) * G_SmithGGX(wi, wo, normal, roughness);
            float nominator = G_SmithGGX(wi, wo, normal, roughness);

            return nominator / (float)denominator;
        }

        //public override float F(Vector3 wo, Vector3 wi, Vector3 position, Vector3 normal, float roughness)
        //{
        //    if (roughness < 0.001f)
        //        roughness = 0.001f;
        //    if (roughness > 1.0f)
        //        roughness = 1.0f;

        //    double denominator = 4.0 * Math.Max(Vector3.Dot(normal, wi), 0.0) * Math.Max(Vector3.Dot(normal, wo), 0.0) + 0.001;

        //    float nominator = D_GGX((wi + wo).normalized, normal, roughness) * G_SmithGGX(wi, wo, normal, roughness);

        //    return nominator / (float)denominator;
        //}

        //public override float SampleF(SamplerBase sampler, Vector3 wo, Vector3 position, Vector3 normal, float roughness, out Vector3 wi, out float pdf)
        //{
        //    //Vector3 w = normal;
        //    //Vector3 sp = sampler.SampleHemiSphere(roughness);
        //    //Vector3 u = Vector3.Cross(new Vector3(0.00424f, 1, 0.00764f), w);
        //    //u.Normalize();
        //    //Vector3 v = Vector3.Cross(u, w);
        //    //normal = sp.x * u + sp.y * v + sp.z * w;
        //    //wi = Vector3.Reflect(wo, normal);

        //    //pdf = (float)(sp.z / Math.PI);

        //    //double denominator = 4.0 * Math.Max(Vector3.Dot(normal, wi), 0.0) * Math.Max(Vector3.Dot(normal, wo), 0.0) + 0.001;

        //    //float nominator = G_SmithGGX(wi, wo, normal, roughness);

        //    //return nominator / (float)denominator;
        //    if (roughness < 0.001f)
        //        roughness = 0.001f;
        //    if (roughness > 1.0f)
        //        roughness = 1.0f;

        //    Vector3 reflect = Vector3.Reflect(wo, normal);
        //    Vector3 sp = sampler.SampleHemiSphere(roughness);
        //    wi = Vector3.ONB(reflect, sp);
        //    if (Vector3.Dot(wi, normal) < 0.0)
        //    {
        //        //wi *= -1.0f;
        //        pdf = (float)(sp.z / Math.PI);
        //        return 0.0f;
        //    }
        //    //float sample = 0.9999f;
        //    //float a = roughness * roughness;
        //    //double cos_theta = Math.Sqrt((1.0 - sample) / (1.0 + (a * a - 1.0) * sample));

        //    //double area = -cos_theta * 2.0 * Math.PI - (-2.0 * Math.PI);
        //    //pdf = 1.0f / (float)area;
        //    //pdf = 1.0f;
        //    pdf = (float)(sp.z / Math.PI);

        //    double denominator = 4.0 * Math.Max(Vector3.Dot(normal, wi), 0.0) * Math.Max(Vector3.Dot(normal, wo), 0.0) + 0.001;
        //    //float nominator = D_GGX((wi + wo).normalized, normal, roughness) * G_SmithGGX(wi, wo, normal, roughness);
        //    float nominator = G_SmithGGX(wi, wo, normal, roughness);

        //    return nominator / (float)denominator;
        //}

        private float D_GGX(Vector3 half, Vector3 normal, float roughness)
        {
            float NdotH = (float)Vector3.Dot(half, normal);

            float root = roughness / (NdotH * NdotH * (roughness * roughness - 1.0f) + 1.0f);

            return (float)(1.0f / Math.PI) * (root * root);
        }

        private float G_SmithGGX(Vector3 inDir, Vector3 outDir, Vector3 normal, float roughness)
        {
            float NdotV = (float)Math.Max(0, Vector3.Dot(inDir, normal));
            float NdotL = (float)Math.Max(0, Vector3.Dot(outDir, normal));

            float ggx2 = GeometrySchlickGGX(NdotV, roughness);
            float ggx1 = GeometrySchlickGGX(NdotL, roughness);

            return ggx1 * ggx2;
        }

        private float GeometrySchlickGGX(float NdotV, float roughness)
        {
            float r = (roughness + 1.0f);
            float k = (r * r) / 8.0f;

            float nom = NdotV;
            float denom = NdotV * (1.0f - k) + k;

            return nom / denom;
        }
        
    }
}
