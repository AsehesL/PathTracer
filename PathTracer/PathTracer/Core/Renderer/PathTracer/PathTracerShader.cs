using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    class PathTracerShader
    {
        //struct MaterialProperty
        //{
        //    public Color albedo;
        //    public Vector3 worldNormal;
        //    public float opacity;
        //    public float metallic;
        //    public float roughness;
        //}
        private enum ShadingType
        {
            Diffuse,
            Reflect,
            Refract,
        }

        private BRDF m_LambertaianBRDF = new LambertatianBRDF();

        private BRDF m_DisneyDiffuseBRDF = new DisneyDiffuseBRDF();

        private CookTorranceBRDF m_CookTorranceBRDF = new CookTorranceBRDF();

        private PathTracer m_PathTracer;
        public PathTracerShader(PathTracer pathTracer)
        {
            m_PathTracer = pathTracer;
        }

        public Color Shade(Material material, SamplerBase sampler, Ray ray, RayCastHit hit)
        {
            if (hit.depth > m_PathTracer.tracingTimes)
            {
                if (material.IsEmissive() && m_PathTracer.sampleDirectLight && ray.isDiffuseRay)
                    return Color.black;
                return material.GetEmissive(hit);
            }
            if (material.IsEmissive())
            {
                if (m_PathTracer.sampleDirectLight && hit.depth != 0 && ray.isDiffuseRay)
                    return Color.black;
                return material.GetEmissive(hit);
            }

            float refractive = material.GetRefractive(hit);
            float metallic = material.GetMetallic(hit);
            float roughness = material.GetRoughness(hit);
            float opacity = material.GetOpacity(hit); 
            Color albedo = material.GetAlbedo(hit);
            Vector3 worldNormal = material.GetWorldNormal(hit);

            float ndv = (float)Math.Max(0.0f, Vector3.Dot(worldNormal, -1.0 * ray.direction));

            if (hit.isBackFace)
                refractive = 1.0f / refractive;

            float F0 = (1.0f - refractive) / (1.0f + refractive);
            F0 = F0 * F0;
            if (F0 > 0.04f)
                F0 = 0.04f;
            Color F0Col = Color.Lerp(new Color(F0, F0, F0), albedo, metallic);

            Color fresnel = Fresnel.FresnelSchlick(ndv, F0Col, roughness);

            float fl = fresnel.Average();

            ShadingType shadingType = ShadingType.Reflect;

            double russianRoulette = sampler.GetRandom();
            if (fl > russianRoulette)
            {
                shadingType = ShadingType.Reflect;
            }
            else if (fl + (1.0f - fl) * (1.0f - opacity) > russianRoulette)
            {
                shadingType = ShadingType.Refract;
            }
            else
            {
                shadingType = ShadingType.Diffuse;
            }

            if (shadingType == ShadingType.Diffuse)
            {
                if (1.0f - metallic < float.Epsilon)
                    return Color.black;

                if (albedo.IsCloseToZero())
                    return Color.black;

                Vector3 sp = sampler.SampleHemiSphere();
                Vector3 wi = Vector3.ONB(worldNormal, sp);
                float pdf = (float)(sp.z / Math.PI);

                double ndl = Vector3.Dot(worldNormal, wi);

                if (ndl <= 0.0)
                    return Color.black;

                float brdf = (roughness < float.Epsilon ? m_LambertaianBRDF : m_DisneyDiffuseBRDF).SampleBRDF(sampler, ray.direction * -1.0, wi, hit.hit, worldNormal, roughness);

                if (brdf < float.Epsilon)
                    return Color.black;

                Ray diffuseRay = new Ray(hit.hit, wi, true);

                Color L = m_PathTracer.PathTracing(diffuseRay, sampler, hit.depth + 1);

                return albedo * L * brdf * (1.0f - metallic) * (float)ndl / pdf;
            }
            if (shadingType == ShadingType.Reflect)
            {
                if (albedo.IsCloseToZero())
                    return Color.black;

                Vector3 sp = sampler.SampleHemiSphere(roughness);
                Vector3 spnormal = Vector3.ONB(worldNormal, sp);
                Vector3 wo = ray.direction * -1.0;
                Vector3 wi = Vector3.Reflect(wo, spnormal);
                float pdf = (float)(sp.z / Math.PI);

                double ndl = Vector3.Dot(worldNormal, wi);

                if (ndl <= 0.0)
                    return Color.black;

                float brdf = m_CookTorranceBRDF.SampleBRDF(sampler, wo, wi, hit.hit, worldNormal, roughness);

                if (brdf < float.Epsilon)
                    return Color.black;

                Ray specularRay = new Ray(hit.hit, wi, false);

                Color L = m_PathTracer.PathTracing(specularRay, sampler, hit.depth + 1);

                return Color.Lerp(Color.white, fresnel, metallic) * L * brdf * (float)ndl / pdf;
            }
            if (shadingType == ShadingType.Refract)
            {

            }
            return Color.black;

            //MaterialProperty property;

            //property.worldNormal = material.GetWorldNormal(hit);
            //property.opacity = material.GetOpacity(hit);
            //property.metallic = material.GetMetallic(hit);
            //property.roughness = material.GetRoughness(hit);
            //property.albedo = material.GetAlbedo(hit);

            //if (m_PathTracer.sampleDirectLight)
            //    result += ShadeDirectLights(property, sampler, ray, hit);

            //result += material.GetOcclusion(hit) * ShadeAmbientLight(property, sampler, ray, hit);

            //return result + material.GetEmissive(hit);
        }

        //private Color RenderAmbientLight(Material material, SamplerBase sampler, Ray ray, RenderChannel renderChannel, RayCastHit hit, Vector3 worldNormal, int depth)
        //{
        //    float opacity = material.GetOpacity(hit);
        //    if (opacity >= 1.0f - float.Epsilon || (opacity > float.Epsilon && sampler.GetRandom() < opacity))
        //    {
        //        // 不透明
        //        float metallic = material.GetMetallic(hit);
        //        float ndv = (float)Math.Max(0.0f, Vector3.Dot(worldNormal, -1.0 * ray.direction));
        //        if (metallic >= 1.0f - float.Epsilon || (metallic > float.Epsilon && sampler.GetRandom() < metallic))
        //        {
        //            Color albedo = GetAlbedoForRender(material, renderChannel, hit, depth);
        //            float roughness = material.GetRoughness(hit);
        //            Vector3 F = BRDF.FresnelSchlickRoughness(ndv, new Vector3(albedo.r, albedo.g, albedo.b), roughness);
        //            Vector3 SP = sampler.SampleHemiSphere(roughness);
        //            Vector3 N = ONB(worldNormal, SP);
        //            Vector3 L = Vector3.Reflect(ray.direction * -1, N);
        //            Ray lray = new Ray(hit.hit, L);
        //            Color col = m_PathTracer.PathTracing(lray, sampler, depth + 1);
        //            float ndl = (float)Math.Max(Vector3.Dot(worldNormal, L), 0.0);
        //            return new Color(col.r * (float)F.x, col.g * (float)F.y, col.b * (float)F.z, col.a) * SpecularBRDF.ApplyBRDF(-1.0 * ray.direction, L, worldNormal, roughness) * ndl;
        //        }
        //        else
        //        {
        //            float roughness = material.GetRoughness(hit);
        //            float F = BRDF.FresnelSchlickRoughness(ndv, 0.04f, roughness);
        //            if (sampler.GetRandom() < F)
        //            {
        //                //Specular
        //                Vector3 SP = sampler.SampleHemiSphere(roughness);
        //                Vector3 N = ONB(worldNormal, SP);
        //                Vector3 L = Vector3.Reflect(ray.direction * -1, N);
        //                Ray lray = new Ray(hit.hit, L);
        //                float ndl = (float)Math.Max(Vector3.Dot(worldNormal, L), 0.0);
        //                return m_PathTracer.PathTracing(lray, sampler, depth + 1) * SpecularBRDF.ApplyBRDF(-1.0 * ray.direction, L, worldNormal, roughness) * ndl;
        //            }
        //            else
        //            {
        //                //Diffuse
        //                Color albedo = GetAlbedoForRender(material, renderChannel, hit, depth);
        //                Vector3 w = worldNormal;
        //                Vector3 u = Vector3.Cross(new Vector3(0.00424f, 1, 0.00764f), w);
        //                u.Normalize();
        //                Vector3 v = Vector3.Cross(u, w);
        //                Vector3 sp = sampler.SampleHemiSphere(1);

        //                Vector3 wi = sp.x * u + sp.y * v + sp.z * w;
        //                wi.Normalize();

        //                float ndl = (float)Math.Max(0.0f, Vector3.Dot(worldNormal, wi));

        //                Ray lray = new Ray(hit.hit, wi);
        //                return ndl * albedo * m_PathTracer.PathTracing(lray, sampler, depth + 1) * DiffuseBRDF.ApplyBRDF(-1.0 * ray.direction, wi, worldNormal, roughness);// / GetPDF((float)sp.z);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //Transparency
        //        float refractive = material.GetRefractive(hit);
        //        float roughness = material.GetRoughness(hit);
        //        Color albedo = GetAlbedoForRender(material, renderChannel, hit, depth);
        //        Vector3 SP = sampler.SampleHemiSphere(roughness);
        //        Vector3 N = ONB(worldNormal, SP);
        //        float et;
        //        Vector3 n;
        //        float cosine;
        //        float reflectProb;
        //        float ndv = (float)Vector3.Dot(ray.direction, N);
        //        if (ndv > 0)
        //        {
        //            n = -1 * N;
        //            et = refractive;
        //            cosine = refractive * ndv;

        //        }
        //        else
        //        {
        //            n = N;
        //            et = 1.0f / refractive;
        //            cosine = -ndv;
        //        }

        //        Vector3 refrac;
        //        if (Vector3.Refract(ray.direction, n, et, out refrac))
        //        {
        //            reflectProb = BRDF.FresnelSchlickRoughnessRefractive(cosine, refractive, roughness);
        //        }
        //        else
        //        {
        //            reflectProb = 1.0f;
        //        }

        //        if (sampler.GetRandom() < reflectProb)
        //        {
        //            Ray lray = new Ray(hit.hit, Vector3.Reflect(ray.direction * -1, N));
        //            return m_PathTracer.PathTracing(lray, sampler, depth + 1);
        //        }
        //        else
        //        {
        //            Ray lray = new Ray(hit.hit, refrac);
        //            return albedo * m_PathTracer.PathTracing(lray, sampler, depth + 1);
        //        }
        //    }
        //}

        //private Color RenderSunLight(Material material, SamplerBase sampler, Ray ray, RenderChannel renderChannel, RayCastHit hit, Vector3 worldNormal, int depth)
        //{
        //    float opacity = material.GetOpacity(hit);
        //    if (opacity >= 1.0f - float.Epsilon || (opacity > float.Epsilon && sampler.GetRandom() < opacity))
        //    {
        //        // 不透明
        //        float metallic = material.GetMetallic(hit);
        //        float ndv = (float)Math.Max(0.0f, Vector3.Dot(worldNormal, -1.0 * ray.direction));
        //        Vector3 L = -1.0 * m_PathTracer.scene.sunLight.SampleSunDirection(sampler);
        //        if (metallic >= 1.0f - float.Epsilon || (metallic > float.Epsilon && sampler.GetRandom() < metallic))
        //        {
        //            Color albedo = GetAlbedoForRender(material, renderChannel, hit, depth);
        //            float roughness = material.GetRoughness(hit);
        //            Vector3 F = BRDF.FresnelSchlickRoughness(ndv, new Vector3(albedo.r, albedo.g, albedo.b), roughness);

        //            double ndl = Vector3.Dot(worldNormal, L);
        //            if (ndl < 0.0)
        //                return Color.black;
        //            Ray lray = new Ray(hit.hit, L);
        //            bool shadow = m_PathTracer.ShadowTracing(lray);
        //            if (shadow)
        //                return Color.black;
        //            ndl = Math.Max(ndl, 0.0);
        //            return m_PathTracer.scene.sunLight.GetSunLightColor() * new Color((float)F.x, (float)F.y, (float)F.z, 1.0f) * SpecularBRDF.ABRDFDirectional(-1.0 * ray.direction, L, worldNormal, roughness) * (float)ndl;
        //        }
        //        else
        //        {
        //            Color albedo = GetAlbedoForRender(material, renderChannel, hit, depth);
        //            float roughness = material.GetRoughness(hit);
        //            float F = BRDF.FresnelSchlickRoughness(ndv, 0.04f, roughness);
        //            if (sampler.GetRandom() < F)
        //            {
        //                //Specular
        //                double ndl = Vector3.Dot(worldNormal, L);
        //                if (ndl < 0.0)
        //                    return Color.black;
        //                Ray lray = new Ray(hit.hit, L);
        //                bool shadow = m_PathTracer.ShadowTracing(lray);
        //                if (shadow)
        //                    return Color.black;
        //                ndl = Math.Max(ndl, 0.0);
        //                return m_PathTracer.scene.sunLight.GetSunLightColor() * SpecularBRDF.ABRDFDirectional(-1.0 * ray.direction, L, worldNormal, roughness) * (float)ndl;
        //            }
        //            else
        //            {
        //                //Diffuse
        //                double ndl = Vector3.Dot(worldNormal, L);
        //                if (ndl < 0.0)
        //                    return Color.black;
        //                Ray lray = new Ray(hit.hit, L);
        //                bool shadow = m_PathTracer.ShadowTracing(lray);
        //                if (shadow)
        //                    return Color.black;
        //                ndl = Math.Max(ndl, 0.0);
        //                return albedo * m_PathTracer.scene.sunLight.GetSunLightColor() * DiffuseBRDF.ABRDFDirectional(-1.0 * ray.direction, L, worldNormal, roughness) * (float)ndl;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        float refractive = material.GetRefractive(hit);
        //        float roughness = material.GetRoughness(hit);

        //        Vector3 SP = sampler.SampleHemiSphere(roughness);
        //        Vector3 N = ONB(worldNormal, SP);
        //        float et;
        //        Vector3 n;
        //        float cosine;
        //        float reflectProb;
        //        float ndv = (float)Vector3.Dot(ray.direction, N);
        //        if (ndv > 0)
        //        {
        //            n = -1 * N;
        //            et = refractive;
        //            cosine = refractive * ndv;

        //        }
        //        else
        //        {
        //            n = N;
        //            et = 1.0f / refractive;
        //            cosine = -ndv;
        //        }

        //        Vector3 refrac;
        //        if (Vector3.Refract(ray.direction, n, et, out refrac))
        //        {
        //            reflectProb = BRDF.FresnelSchlickRoughnessRefractive(cosine, refractive, roughness);
        //        }
        //        else
        //        {
        //            reflectProb = 1.0f;
        //        }

        //        if (sampler.GetRandom() < reflectProb)
        //        {
        //            Vector3 L = -1.0 * m_PathTracer.scene.sunLight.SampleSunDirection(sampler);
        //            double ndl = Vector3.Dot(worldNormal, L);
        //            if (ndl < 0.0)
        //                return Color.black;
        //            Ray lray = new Ray(hit.hit, L);
        //            bool shadow = m_PathTracer.ShadowTracing(lray);
        //            if (shadow)
        //                return Color.black;
        //            ndl = Math.Max(ndl, 0.0);
        //            return m_PathTracer.scene.sunLight.GetSunLightColor() * SpecularBRDF.ABRDFDirectional(-1.0 * ray.direction, L, worldNormal, roughness) * (float)ndl;
        //        }
        //        else
        //        {
        //            return Color.black;
        //        }
        //    }
        //}
    }

    class PathTracerSkyShader
    {
        private PathTracer m_PathTracer;


        public PathTracerSkyShader(PathTracer pathTracer)
        {
            m_PathTracer = pathTracer;
        }

        public Color Shade(Material skyMaterial, SamplerBase sampler, Ray ray)
        {
            if (skyMaterial == null)
                return Color.black;

            RayCastHit hit = default(RayCastHit);
            hit.normal = ray.direction;

            return skyMaterial.GetEmissive(hit);
        }
    }
}
