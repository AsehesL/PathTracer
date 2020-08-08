using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    /// <summary>
    /// 大气散射
    /// </summary>
    [ShaderType("Atmosphere")]
    class Atmosphere : Sky
    {
        public float numSunSamples;
        public float rayleighScatterCoef; //rayleigh散射系数
        public float mieScatterCoef; //mie散射系数
        public float rayleighExtinctionCoef; //rayleigh消光系数
        public float mieExtinctionCoef; //mie消光系数
        public float sunIntensityScale; 

        private Color m_BR = new Color(5.8e-6f, 13.5e-6f, 33.1e-6f);
        private Color m_BM = Color.white * 21e-6f;
        //private Color m_BetaM = Color.white * 0.0001f;

        /// <summary>
        /// 地球半径
        /// </summary>
        private float m_EarthRadius = 6360e3f;
        /// <summary>
        /// 大气层半径
        /// </summary>
        private float m_AtmosphereRadius = 6420e3f;

        private float m_HR = 7994;
        private float m_HM = 1220;

        private float m_Param0;
        private float m_Param1;
        private float m_Param2;
        private float m_Param3;
        private float m_Param4;
        private float m_Param5;


        public Atmosphere(SunLight sunLight) : base(sunLight)
        {
            m_Param0 = 3.0f / (16.0f * (float)Math.PI);
            m_Param1 = 3.0f / (8.0f * (float)Math.PI);
            m_Param2 = (1.0f - kMieG * kMieG);
            m_Param3 = (2.0f + kMieG * kMieG);
            m_Param4 = 1.0f + kMieG * kMieG;
            m_Param5 = 2.0f * kMieG;
        }

        private class AtmosphereScatteringState : IRenderStateObject
        {
            public float opticalDepthR = 0;
            public float opticalDepthM = 0;

            public void ResetState()
            {
                opticalDepthR = 0;
                opticalDepthM = 0;
            }
        }

        public override Color RenderColor(Tracer tracer, SamplerBase sampler, RenderState renderState, Ray ray, RayCastHit hit, int currentDepth, bool isHitting)
        {
            if (hasSunLight == false)
                return Color.black;

            if (isHitting)
            {
                if (!shouldRenderParticipatingMedia || currentDepth != 0)
                    return Color.black;
            }

            Vector3 oripos = new Vector3(0, ray.origin.y + m_EarthRadius, 0);

            float dis = currentDepth == 0 && isHitting ? (float)hit.distance : RayCastSphere(ray.direction, oripos, m_AtmosphereRadius);

            float stepLength = dis / sampler.numSamples;

            AtmosphereScatteringState state = renderState.GetRenderStateObject<AtmosphereScatteringState>(GetType());

            Vector3 pos = oripos + ray.direction * (stepLength * (0.5f + sampler.currentSample));
            float height = (float)pos.magnitude - m_EarthRadius;
            float hr = (float)Math.Exp(-height / m_HR) * stepLength;
            float hm = (float)Math.Exp(-height / m_HM) * stepLength;

            state.opticalDepthR += hr;
            state.opticalDepthM += hm;

            Vector3 sunDir = -1.0 * GetSunDirection(sampler);

            if (currentDepth == 0) //只有在深度为0时才计算shadow
            {
                Ray shadowRay = new Ray(ray.origin + ray.direction * stepLength * (sampler.currentSample + 0.5f), sunDir);

                if (tracer.TracingOnce(shadowRay))
                {
                    return Color.black;
                }
            }


            Color sumR = Color.black, sumM = Color.black;

            float cosAngle = (float)Vector3.Dot(ray.direction, sunDir); //只计算方向光的大气散射，不额外计算环境中其它光源的散射
            float phaseR = RayleighPhase(cosAngle);
            float phaseM = MiePhase(cosAngle);

            float opticalDepthLightR = 0, opticalDepthLightM = 0;

            int j = 0;
     
            float lightDistance = RayCastSphere(sunDir, pos, m_AtmosphereRadius);
            float segmentLengthLight = lightDistance / numSunSamples, tCurrentLight = 0;
            for (j = 0; j < numSunSamples; j++)
            {
                Vector3 samplePositionLight = pos + (tCurrentLight + segmentLengthLight * 0.5f) * sunDir;
                float heightLight = (float)samplePositionLight.magnitude - m_EarthRadius;
                if (heightLight < 0) break;
                opticalDepthLightR += (float)Math.Exp(-heightLight / m_HR) * segmentLengthLight;
                opticalDepthLightM += (float)Math.Exp(-heightLight / m_HM) * segmentLengthLight;
                tCurrentLight += segmentLengthLight;
            }
            
            if (j == numSunSamples)
            {
                Color tau = m_BR * (state.opticalDepthR + opticalDepthLightR) * rayleighExtinctionCoef + m_BM * 1.1f * (state.opticalDepthM + opticalDepthLightM) * mieExtinctionCoef;
                Color attenuation = new Color((float)Math.Exp(-tau.r), (float)Math.Exp(-tau.g), (float)Math.Exp(-tau.b)); ;
                sumR += attenuation * hr;
                sumM += attenuation * hm;
            }

            return GetSunColor() * sunIntensityScale * (phaseR * m_BR * sumR * rayleighScatterCoef + phaseM * m_BM * mieScatterCoef * sumM) * sampler.numSamples;
        }

        /// <summary>
        /// Mie散射相位方程
        /// </summary>
        /// <param name="cosAngle">视线和光线的夹角</param>
        /// <returns></returns>
        private float MiePhase(float cosAngle)
        {
            return m_Param1 * (m_Param2 * (1.0f + cosAngle * cosAngle)) / (m_Param3 * (float)Math.Pow(m_Param4 - m_Param5 * cosAngle, 1.5f));
        }

        /// <summary>
        /// Rayleigh散射相位方程
        /// </summary>
        /// <param name="cosAngle"></param>
        /// <returns></returns>
        private float RayleighPhase(float cosAngle)
        {
            return m_Param0 * (1 + cosAngle * cosAngle);
        }

        /// <summary>
        /// 判断和球体的相交，用于找到视线在大气层上的交点
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="pos"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private float RayCastSphere(Vector3 direction, Vector3 pos, float radius)
        {

            float vala = (float)Vector3.Dot(direction, direction);
            float valb = (float)Vector3.Dot(direction, pos) * 2.0f;
            float valc = (float)Vector3.Dot(pos, pos) - radius * radius;

            float dis = valb * valb - 4.0f * vala * valc;

            if (dis < 0.0f)
                return 0.0f;
            else
            {
                float e = (float)Math.Sqrt(dis);
                float denom = 2.0f * vala;
                float t = (-valb - e) / denom;

                if (t > float.Epsilon)
                {
                    return t;
                }


                t = (-valb + e) / denom;

                if (t > float.Epsilon)
                {
                    return t;
                }
            }

            return 0.0f;
        }
    }
}
