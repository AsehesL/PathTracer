using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    //[ShaderType("Transparency")]
    //class TransparencyShader : MaterialShader
    //{
    //    public Color color = Color.white;
    //    public float refractive;
    //    public float roughness;
    //    public Vector2 tile;
    //    public Texture bump;

    //    private BRDFBase m_SpecularBRDF = new CookTorranceBRDF();

    //    public override bool ShouldRenderBackFace()
    //    {
    //        return true;
    //    }

    //    public override Color Render(Tracer tracer, SamplerBase sampler, RenderState renderState, Ray ray, RayCastHit hit)
    //    {
    //        Color result = default(Color);

    //        Vector3 worldNormal = bump != null ? RecalucateNormal(hit.normal, hit.tangent, bump.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y))) : hit.normal;

    //        if (tracer.sceneData.sky != null && tracer.sceneData.sky.hasSunLight)
    //        {
    //            //单独计算方向光照
    //            result += RenderDirectionalLighting(tracer, sampler, hit.hit, worldNormal, ray.direction);
    //        }

    //        //计算环境光照
    //        result += RenderAmbientLighting(tracer, sampler, renderState, hit.hit, worldNormal, ray.direction, hit.depth);

    //        return result;
    //    }

    //    public override Color RenderEmissiveOnly(Tracer tracer, SamplerBase sampler, RenderState renderState, Ray ray, RayCastHit hit)
    //    {
    //        return Color.black;
    //    }

    //    public override Color RenderPreviewChannel(Tracer tracer, SamplerBase sampler, Ray ray, RayCastHit hit, RenderChannel renderChannel)
    //    {
    //        switch (renderChannel)
    //        {
    //            case RenderChannel.WorldNormal:
    //                {
    //                    Vector3 worldNormal = hit.normal;
    //                    return new Color((float)worldNormal.x * 0.5f + 0.5f, (float)worldNormal.y * 0.5f + 0.5f, (float)worldNormal.z * 0.5f + 0.5f);
    //                }
    //            case RenderChannel.Albedo:
    //                {
    //                    return color;
    //                }
    //            case RenderChannel.Occlusion:
    //                return Color.white;
    //        }
    //        return Color.black;
    //    }

    //    private Color RenderDirectionalLighting(Tracer tracer, SamplerBase sampler, Vector3 worldPoint, Vector3 worldNormal, Vector3 viewDirection)
    //    {
    //        //Transparency

    //        Vector3 N = ImportanceGGXDirection(worldNormal, sampler, roughness);
    //        float et;
    //        Vector3 n;
    //        float cosine;
    //        float reflectProb;
    //        float ndv = (float)Vector3.Dot(viewDirection, N);
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
    //        if (Vector3.Refract(viewDirection, n, et, out refrac))
    //        {
    //            reflectProb = FresnelSchlickRoughnessRefractive(cosine, refractive, roughness);
    //        }
    //        else
    //        {
    //            reflectProb = 1.0f;
    //        }

    //        if (sampler.GetRandom() < reflectProb)
    //        {
    //            Vector3 L = -1.0 * tracer.sceneData.sky.GetSunDirection(sampler);
    //            double ndl = Vector3.Dot(worldNormal, L);
    //            if (ndl < 0.0)
    //                return Color.black;
    //            Ray lray = new Ray(worldPoint, L);
    //            bool shadow = tracer.TracingOnce(lray);
    //            if (shadow)
    //                return Color.black;
    //            ndl = Math.Max(ndl, 0.0);
    //            return tracer.sceneData.sky.GetSunColor() * m_SpecularBRDF.BRDFDirectional(-1.0 * viewDirection, L, worldNormal, roughness) * (float)ndl;
    //        }
    //        else
    //        {
    //            return Color.black;
    //        }
    //    }

    //    private Color RenderAmbientLighting(Tracer tracer, SamplerBase sampler, RenderState renderState, Vector3 worldPoint, Vector3 worldNormal, Vector3 viewDirection, int depth)
    //    {
    //        //Transparency
    //        Vector3 N = ImportanceGGXDirection(worldNormal, sampler, roughness);
    //        float et;
    //        Vector3 n;
    //        float cosine;
    //        float reflectProb;
    //        float ndv = (float)Vector3.Dot(viewDirection, N);
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
    //        if (Vector3.Refract(viewDirection, n, et, out refrac))
    //        {
    //            reflectProb = FresnelSchlickRoughnessRefractive(cosine, refractive, roughness);
    //        }
    //        else
    //        {
    //            reflectProb = 1.0f;
    //        }

    //        if (sampler.GetRandom() < reflectProb)
    //        {
    //            Ray lray = new Ray(worldPoint, Vector3.Reflect(viewDirection * -1, N));
    //            return tracer.Tracing(lray, sampler, renderState, depth + 1);
    //        }
    //        else
    //        {
    //            Ray lray = new Ray(worldPoint, refrac);
    //            return color * tracer.Tracing(lray, sampler, renderState, depth + 1);
    //        }
    //    }
    //}
}
