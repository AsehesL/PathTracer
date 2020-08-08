using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    /// <summary>
    /// PBR光照模型
    /// </summary>
    public enum PBRShadingModel
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default,
        /// <summary>
        /// 透明
        /// </summary>
        Transparent,
        /// <summary>
        /// 双面薄透光材质
        /// </summary>
        DoubleSidedTranslucent,
        /// <summary>
        /// 车漆
        /// </summary>
        CarPaint,
    }

    interface IPBRProperty
    {
        Color GetAlbedo();
        float GetRoughness();
        float GetRoughness2();
        float GetMetallic();
        Color GetEmissive();
        float GetRefractive();
        bool TangentSpaceNormal();
        Color GetNormal();
        float GetOcclusion();
        float GetDoubleSidedTransmittance();
        Color GetDoubleSidedTransmissionColor();
    }

    abstract class PBRShaderBase<T> : Shader where T : IPBRProperty
    {
        public abstract PBRShadingModel shadingModel { get; }

        /// <summary>
        /// 是否开启透明裁剪
        /// </summary>
        public bool transparentCutOut;

        protected abstract T SampleProperty(Ray ray, RayCastHit hit);

        protected abstract BRDFBase SpecularBRDF { get; }

        protected abstract BRDFBase DiffuseBRDF { get; }

        public override bool ShouldCull(Ray ray, RayCastHit hit)
        {
            if (!transparentCutOut)
                return false;
            if (GetTransparentCutOutAlpha(ray, hit) < 0.5f)
                return true;
            return false;
        }

        protected abstract float GetTransparentCutOutAlpha(Ray ray, RayCastHit hit);

        public override Color RenderEmissiveOnly(Tracer tracer, SamplerBase sampler, RenderState renderState, Ray ray, RayCastHit hit)
        {
            T property = SampleProperty(ray, hit);

            return property.GetEmissive();
        }

        public override Color Render(Tracer tracer, SamplerBase sampler, RenderState renderState, Ray ray, RayCastHit hit)
        {

            T property = SampleProperty(ray, hit);

            //计算世界空间法线（可能需要根据切空间法线重新计算法线）
            Vector3 worldNormal = property.TangentSpaceNormal() ? RecalucateNormal(hit.normal, hit.tangent, property.GetNormal()) : hit.normal;

            Color result = default(Color);

            if(tracer.sceneData.sky != null && tracer.sceneData.sky.hasSunLight)
            {
                //单独计算方向光照
                result += RenderDirectionalLighting(tracer, sampler, ray, hit.hit, worldNormal, property);
            }

            //计算环境光照
            result += property.GetOcclusion() * RenderAmbientLighting(tracer, sampler, renderState, ray, hit.hit, worldNormal, property, hit.depth);

            return result + property.GetEmissive();
        }

        private Color RenderDirectionalLighting(Tracer tracer, SamplerBase sampler, Ray ray, Vector3 worldPoint, Vector3 worldNormal, T property)
        {
            Color albedo = property.GetAlbedo();
            float roughness = property.GetRoughness();
            if (shadingModel != PBRShadingModel.Transparent || sampler.GetRandom() < albedo.a)
            {
                if(shadingModel == PBRShadingModel.CarPaint)
                {
                    // Car Paint
                    float ndv = (float)Math.Max(0.0f, Vector3.Dot(worldNormal, -1.0 * ray.direction));
                    
                    float F = FresnelSchlickRoughness(ndv, 0.04f, roughness);
                    if (sampler.GetRandom() < F)
                    {
                        Vector3 L = -1.0 * tracer.sceneData.sky.GetSunDirection(sampler);
                        double ndl = Vector3.Dot(worldNormal, L);
                        if (ndl < 0.0)
                            return Color.black;
                        Ray lray = new Ray(worldPoint, L);
                        bool shadow = tracer.TracingOnce(lray);
                        if (shadow)
                            return Color.black;
                        ndl = Math.Max(ndl, 0.0);
                        return tracer.sceneData.sky.GetSunColor() * SpecularBRDF.BRDFDirectional(-1.0 * ray.direction, L, worldNormal, roughness) * (float)ndl;
                    }
                    else
                    {
                        float roughness2 = property.GetRoughness2();
                        Vector3 MetalF = FresnelSchlickRoughness(ndv, new Vector3(albedo.r, albedo.g, albedo.b), roughness2);
                        Vector3 L = -1.0 * tracer.sceneData.sky.GetSunDirection(sampler);
                        double ndl = Vector3.Dot(worldNormal, L);
                        if (ndl < 0.0)
                            return Color.black;
                        Ray lray = new Ray(worldPoint, L);
                        bool shadow = tracer.TracingOnce(lray);
                        if (shadow)
                            return Color.black;
                        ndl = Math.Max(ndl, 0.0);
                        return tracer.sceneData.sky.GetSunColor() * new Color((float)MetalF.x, (float)MetalF.y, (float)MetalF.z, 1.0f) * SpecularBRDF.BRDFDirectional(-1.0 * ray.direction, L, worldNormal, roughness2) * (float)ndl;
                    }
                }
                {
                    //Opaque
                    if (shadingModel == PBRShadingModel.DoubleSidedTranslucent && sampler.GetRandom() < property.GetDoubleSidedTransmittance())
                    {
                        //Translucent
                        worldNormal *= -1;
                        worldPoint += worldNormal * tracer.epsilon * 3.0;

                        Vector3 L = -1.0 * tracer.sceneData.sky.GetSunDirection(sampler);
                        double ndl = Vector3.Dot(worldNormal, L);
                        if (ndl < 0.0)
                            return Color.black;
                        Ray lray = new Ray(worldPoint, L);
                        bool shadow = tracer.TracingOnce(lray);
                        if (shadow)
                            return Color.black;
                        ndl = Math.Max(ndl, 0.0);
                        return albedo * property.GetDoubleSidedTransmissionColor() * tracer.sceneData.sky.GetSunColor() * DiffuseBRDF.BRDFDirectional(-1.0 * ray.direction, L, worldNormal, roughness) * (float)ndl;
                    }
                    float ndv = (float)Math.Max(0.0f, Vector3.Dot(worldNormal, -1.0 * ray.direction));
                    if (shadingModel != PBRShadingModel.DoubleSidedTranslucent && sampler.GetRandom() < property.GetMetallic())
                    {
                        //Metallic
                        Vector3 F = FresnelSchlickRoughness(ndv, new Vector3(albedo.r, albedo.g, albedo.b), roughness);
                        Vector3 L = -1.0 * tracer.sceneData.sky.GetSunDirection(sampler);
                        double ndl = Vector3.Dot(worldNormal, L);
                        if (ndl < 0.0)
                            return Color.black;
                        Ray lray = new Ray(worldPoint, L);
                        bool shadow = tracer.TracingOnce(lray);
                        if (shadow)
                            return Color.black;
                        ndl = Math.Max(ndl, 0.0);
                        return tracer.sceneData.sky.GetSunColor() * new Color((float)F.x, (float)F.y, (float)F.z, 1.0f) * SpecularBRDF.BRDFDirectional(-1.0 * ray.direction, L, worldNormal, roughness) * (float)ndl;
                    }
                    else
                    {
                        //Dielectric
                        float F = FresnelSchlickRoughness(ndv, 0.04f, roughness);
                        if (sampler.GetRandom() < F)
                        {
                            //Specular
                            Vector3 L = -1.0 * tracer.sceneData.sky.GetSunDirection(sampler);
                            double ndl = Vector3.Dot(worldNormal, L);
                            if (ndl < 0.0)
                                return Color.black;
                            Ray lray = new Ray(worldPoint, L);
                            bool shadow = tracer.TracingOnce(lray);
                            if (shadow)
                                return Color.black;
                            ndl = Math.Max(ndl, 0.0);
                            return tracer.sceneData.sky.GetSunColor() * SpecularBRDF.BRDFDirectional(-1.0 * ray.direction, L, worldNormal, roughness) * (float)ndl;
                        }
                        else
                        {
                            //Diffuse
                            Vector3 L = -1.0 * tracer.sceneData.sky.GetSunDirection(sampler);
                            double ndl = Vector3.Dot(worldNormal, L);
                            if (ndl < 0.0)
                                return Color.black;
                            Ray lray = new Ray(worldPoint, L);
                            bool shadow = tracer.TracingOnce(lray);
                            if (shadow)
                                return Color.black;
                            ndl = Math.Max(ndl, 0.0);
                            return albedo * tracer.sceneData.sky.GetSunColor() * DiffuseBRDF.BRDFDirectional(-1.0 * ray.direction, L, worldNormal, roughness) * (float)ndl;
                        }
                    }
                }
            }
            else
            {
                //Transparency
                float refractive = property.GetRefractive();
                
                Vector3 N = ImportanceGGXDirection(worldNormal, sampler, roughness);
                float et;
                Vector3 n;
                float cosine;
                float reflectProb;
                float ndv = (float)Vector3.Dot(ray.direction, N);
                if (ndv > 0)
                {
                    n = -1 * N;
                    et = refractive;
                    cosine = refractive * ndv;

                }
                else
                {
                    n = N;
                    et = 1.0f / refractive;
                    cosine = -ndv;
                }

                Vector3 refrac;
                if (Vector3.Refract(ray.direction, n, et, out refrac))
                {
                    reflectProb = FresnelSchlickRoughnessRefractive(cosine, refractive, roughness);
                }
                else
                {
                    reflectProb = 1.0f;
                }

                if (sampler.GetRandom() < reflectProb)
                {
                    Vector3 L = -1.0 * tracer.sceneData.sky.GetSunDirection(sampler);
                    double ndl = Vector3.Dot(worldNormal, L);
                    if (ndl < 0.0)
                        return Color.black;
                    Ray lray = new Ray(worldPoint, L);
                    bool shadow = tracer.TracingOnce(lray);
                    if (shadow)
                        return Color.black;
                    ndl = Math.Max(ndl, 0.0);
                    return tracer.sceneData.sky.GetSunColor() * SpecularBRDF.BRDFDirectional(-1.0 * ray.direction, L, worldNormal, roughness) * (float)ndl;
                }
                else
                {
                    return Color.black;
                }
            }
        }

        private Color RenderAmbientLighting(Tracer tracer, SamplerBase sampler, RenderState renderState, Ray ray, Vector3 worldPoint, Vector3 worldNormal, T property, int depth)
        {
            Color albedo = property.GetAlbedo();
            float roughness = property.GetRoughness();
            if (shadingModel != PBRShadingModel.Transparent || sampler.GetRandom() < albedo.a)
            {
                if (shadingModel == PBRShadingModel.CarPaint)
                {
                    // Car Paint
                    float ndv = (float)Math.Max(0.0f, Vector3.Dot(worldNormal, -1.0 * ray.direction));
                    float F = FresnelSchlickRoughness(ndv, 0.04f, roughness);
                    if (sampler.GetRandom() < F)
                    {
                        Vector3 N = ImportanceGGXDirection(worldNormal, sampler, roughness);
                        Vector3 L = Vector3.Reflect(ray.direction * -1, N);
                        Ray lray = new Ray(worldPoint, L);
                        float ndl = (float)Math.Max(Vector3.Dot(worldNormal, L), 0.0);
                        return tracer.Tracing(lray, sampler, renderState, depth + 1) * SpecularBRDF.BRDF(-1.0 * ray.direction, L, worldNormal, roughness) * ndl;
                    }
                    else
                    {
                        float roughness2 = property.GetRoughness2();
                        Vector3 MetalF = FresnelSchlickRoughness(ndv, new Vector3(albedo.r, albedo.g, albedo.b), roughness2);
                        Vector3 N = ImportanceGGXDirection(worldNormal, sampler, roughness2);
                        Vector3 L = Vector3.Reflect(ray.direction * -1, N);
                        Ray lray = new Ray(worldPoint, L);
                        Color col = tracer.Tracing(lray, sampler, renderState, depth + 1);
                        float ndl = (float)Math.Max(Vector3.Dot(worldNormal, L), 0.0);
                        return new Color(col.r * (float)MetalF.x, col.g * (float)MetalF.y, col.b * (float)MetalF.z, col.a) * SpecularBRDF.BRDF(-1.0 * ray.direction, L, worldNormal, roughness2) * ndl;
                    }
                }
                {
                    //Opaque
                    if (shadingModel == PBRShadingModel.DoubleSidedTranslucent && sampler.GetRandom() < property.GetDoubleSidedTransmittance())
                    {
                        //Translucent
                        worldNormal *= -1;
                        worldPoint += worldNormal * tracer.epsilon * 3.0;

                        Vector3 w = worldNormal;
                        Vector3 u = Vector3.Cross(new Vector3(0.00424f, 1, 0.00764f), w);
                        u.Normalize();
                        Vector3 v = Vector3.Cross(u, w);
                        Vector3 sp = sampler.SampleHemiSphere(0);

                        Vector3 wi = sp.x * u + sp.y * v + sp.z * w;
                        wi.Normalize();

                        float ndl = (float)Math.Max(0.0f, Vector3.Dot(worldNormal, wi));

                        Ray lray = new Ray(worldPoint, wi);
                        return ndl * albedo * property.GetDoubleSidedTransmissionColor() * tracer.Tracing(lray, sampler, renderState, depth + 1) * DiffuseBRDF.BRDF(-1.0 * ray.direction, wi, worldNormal, roughness);
                    }
                    float ndv = (float)Math.Max(0.0f, Vector3.Dot(worldNormal, -1.0 * ray.direction));
                    if (shadingModel != PBRShadingModel.DoubleSidedTranslucent && sampler.GetRandom() < property.GetMetallic())
                    {
                        //Metallic
                        Vector3 F = FresnelSchlickRoughness(ndv, new Vector3(albedo.r, albedo.g, albedo.b), roughness);
                        Vector3 N = ImportanceGGXDirection(worldNormal, sampler, roughness);
                        Vector3 L = Vector3.Reflect(ray.direction * -1, N);
                        Ray lray = new Ray(worldPoint, L);
                        Color col = tracer.Tracing(lray, sampler, renderState, depth + 1);
                        float ndl = (float)Math.Max(Vector3.Dot(worldNormal, L), 0.0);
                        return new Color(col.r * (float)F.x, col.g * (float)F.y, col.b * (float)F.z, col.a) * SpecularBRDF.BRDF(-1.0 * ray.direction, L, worldNormal, roughness) * ndl;
                    }
                    else
                    {
                        //Dielectric
                        float F = FresnelSchlickRoughness(ndv, 0.04f, roughness);
                        if (sampler.GetRandom() < F)
                        {
                            //Specular
                            Vector3 N = ImportanceGGXDirection(worldNormal, sampler, roughness);
                            Vector3 L = Vector3.Reflect(ray.direction * -1, N);
                            Ray lray = new Ray(worldPoint, L);
                            float ndl = (float)Math.Max(Vector3.Dot(worldNormal, L), 0.0);
                            return tracer.Tracing(lray, sampler, renderState, depth + 1) * SpecularBRDF.BRDF(-1.0 * ray.direction, L, worldNormal, roughness) * ndl;
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
                            return ndl * albedo * tracer.Tracing(lray, sampler, renderState, depth + 1) * DiffuseBRDF.BRDF(-1.0 * ray.direction, wi, worldNormal, roughness);
                        }
                    }
                }
            }
            else
            {
                //Transparency
                float refractive = property.GetRefractive();
                Vector3 N = ImportanceGGXDirection(worldNormal, sampler, roughness);
                float et;
                Vector3 n;
                float cosine;
                float reflectProb;
                float ndv = (float)Vector3.Dot(ray.direction, N);
                if (ndv > 0)
                {
                    n = -1 * N;
                    et = refractive;
                    cosine = refractive * ndv;

                }
                else
                {
                    n = N;
                    et = 1.0f / refractive;
                    cosine = -ndv;
                }

                Vector3 refrac;
                if (Vector3.Refract(ray.direction, n, et, out refrac))
                {
                    reflectProb = FresnelSchlickRoughnessRefractive(cosine, refractive, roughness);
                }
                else
                {
                    reflectProb = 1.0f;
                }

                if (sampler.GetRandom() < reflectProb)
                {
                    Ray lray = new Ray(worldPoint, Vector3.Reflect(ray.direction * -1, N));
                    return tracer.Tracing(lray, sampler, renderState, depth + 1);
                }
                else
                {
                    Ray lray = new Ray(worldPoint, refrac);
                    return albedo * tracer.Tracing(lray, sampler, renderState, depth + 1);
                }
            }
        }

        public override Color RenderPreviewChannel(Tracer tracer, Ray ray, RayCastHit hit, RenderChannel renderChannel)
        {
            T property = SampleProperty(ray, hit);
            switch(renderChannel)
            {
                case RenderChannel.Albedo:
                    return property.GetAlbedo();
                case RenderChannel.Metallic:
                    return Color.white * property.GetMetallic();
                case RenderChannel.Roughness:
                    return Color.white * property.GetRoughness();
                case RenderChannel.WorldNormal:
                    {
                        Vector3 worldNormal = property.TangentSpaceNormal() ? RecalucateNormal(hit.normal, hit.tangent, property.GetNormal()) : hit.normal;
                        return new Color((float)worldNormal.x * 0.5f + 0.5f, (float)worldNormal.y * 0.5f + 0.5f, (float)worldNormal.z * 0.5f + 0.5f);
                    }
                case RenderChannel.Emissive:
                    return property.GetEmissive();
                case RenderChannel.Occlusion:
                    return Color.white * property.GetOcclusion();
                case RenderChannel.DirectionalLightShadow:
                    {
                        if (!tracer.sceneData.sky.hasSunLight)
                            return Color.black;
                        Ray shadowray = new Ray(hit.hit, -1.0 * tracer.sceneData.sky.GetSunDirection(null));
                        if (!tracer.TracingOnce(shadowray))
                            return tracer.sceneData.sky.GetSunColor();
                        return Color.black;
                    }
                case RenderChannel.Diffuse:
                    {
                        if (!tracer.sceneData.sky.hasSunLight)
                            return Color.black;
                        Vector3 sun = -1.0 * tracer.sceneData.sky.GetSunDirection(null);
                        Vector3 worldNormal = property.TangentSpaceNormal() ? RecalucateNormal(hit.normal, hit.tangent, property.GetNormal()) : hit.normal;
                        float ndl = (float)Math.Max(0.0, Vector3.Dot(sun, worldNormal));
                        Ray shadowray = new Ray(hit.hit, sun);
                        if (!tracer.TracingOnce(shadowray))
                            return tracer.sceneData.sky.GetSunColor() * property.GetAlbedo() * ndl;
                        return Color.black;
                    }
                case RenderChannel.DiffuseNoLighting:
                    {
                        Vector3 worldNormal = property.TangentSpaceNormal() ? RecalucateNormal(hit.normal, hit.tangent, property.GetNormal()) : hit.normal;
                        float ndv = (float)Math.Max(0.0, Vector3.Dot(-1.0 * ray.direction, worldNormal));
                        return property.GetAlbedo() * ndv;
                    }
            }
            return Color.black;
        }
    }
}
