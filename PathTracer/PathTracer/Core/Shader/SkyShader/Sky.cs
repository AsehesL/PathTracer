using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    ///// <summary>
    ///// 用于天空渲染的shader
    ///// </summary>
    //public abstract class Sky : ShaderBase
    //{
    //    //protected const float kSunRough = 0.12f;
    //    //protected const float kMieG = 0.76f;

    //    //public bool hasSunLight { get { return m_SunLight != null; } }

    //    //public bool shouldRenderParticipatingMedia { get { return m_SunLight != null ? m_SunLight.renderParticipatingMedia : false; } }

    //    //protected SunLight m_SunLight;

    //    public Sky()
    //    {
    //    }

    //    //public virtual Vector3 GetSunDirection(SamplerBase sampler)
    //    //{
    //    //    if (m_SunLight == null)
    //    //        return default(Vector3);
    //    //    if (sampler == null)
    //    //        return m_SunLight.sunDirection;
    //    //    return ImportanceGGXDirection(m_SunLight.sunDirection, sampler, kSunRough);
    //    //}

    //    //public virtual Color GetSunColor()
    //    //{
    //    //    if (m_SunLight == null)
    //    //        return Color.black;
    //    //    return m_SunLight.sunColor * m_SunLight.sunIntensity;
    //    //}

    //    public abstract Color RenderColor(Tracer tracer, SamplerBase sampler, RenderState renderState, Ray ray, RayCastHit hit, int currentDepth, bool isHitting);
    //}
}
