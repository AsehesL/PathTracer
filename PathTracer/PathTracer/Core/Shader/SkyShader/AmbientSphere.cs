using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    [ShaderType("Environment")]
    class AmbientSphere : Sky
    {

        public Texture environment;
        public float intensity = 1.0f;

        public float numExtraSamples;

        public float maxDistance;

        public float scatteringCoef; //散射系数

        public float extinctionCoef; //消光系数

        private float m_Param0;
        private float m_Param1;
        private float m_Param2;
        private float m_Param3;
        private float m_Param4;

        private class AmbientSphereState : IRenderStateObject
        {
            public float extinction = 0;

            public void ResetState()
            {
                extinction = 0;
            }
        }

        public AmbientSphere(SunLight sunLight) : base(sunLight)
        {
            m_Param0 = 2.0f * kMieG;
            m_Param1 = 3.0f / (8.0f * (float)Math.PI);
            m_Param2 = (1.0f - kMieG * kMieG);
            m_Param3 = (2.0f + kMieG * kMieG);
            m_Param4 = 1.0f + kMieG * kMieG;
        }

        public override Color RenderColor(Tracer tracer, SamplerBase sampler, RenderState renderState, Ray ray, RayCastHit hit, int currentDepth, bool isHitting)
        {
            Color sky = default(Color);
            if (!isHitting) 
                sky += RenderSkyColor(ray); //如果射线检测失败，直接渲染天空颜色

            if(currentDepth == 0 && shouldRenderParticipatingMedia) //只有在射线检测深度为0时才计算参与介质
            {
                float dis = isHitting ? (float)hit.distance : maxDistance;
                if (dis > maxDistance)
                    dis = maxDistance;
                if (dis < 0.0f)
                    dis = 0.0f;

                int exsamples = 1 + (int)numExtraSamples; //额外增加采样次数，提高计算精度
                int numSamples = sampler.numSamples * exsamples;
                float step = dis / numSamples;

                AmbientSphereState state = renderState.GetRenderStateObject<AmbientSphereState>(GetType());

                Color light = default(Color);
                for (int i = 0; i < exsamples; i++)
                {
                    float scattering = scatteringCoef * step;
                    state.extinction += extinctionCoef * step;

                    float coef = scattering * (float)Math.Exp(state.extinction);

                    int csample = sampler.currentSample * exsamples + i;
                    Vector3 pos = ray.origin + ray.direction * step * (csample + 0.5f);
                    Vector3 shadowdir = sampler.SampleSphere();
                    Ray shadowRay = new Ray(pos, shadowdir);
                    RayCastHit shadowhit;
                    Color ambient = default(Color);
                    if(tracer.TracingOnce(shadowRay, out shadowhit))
                    {
                        ambient = shadowhit.shader != null ? shadowhit.shader.RenderEmissiveOnly(tracer, sampler, renderState, shadowRay, shadowhit) : Color.black;
                    }
                    else
                    {
                        ambient = RenderSkyColor(shadowRay);
                    }
                    light += ambient * coef * Mie((float)Vector3.Dot(ray.direction, shadowRay.direction));
                    if(hasSunLight)
                    {
                        shadowdir = -1.0 * GetSunDirection(sampler);
                        shadowRay.direction = shadowdir;
                        if(!tracer.TracingOnce(shadowRay))
                        {
                            light += GetSunColor() * coef * Mie((float)Vector3.Dot(ray.direction, shadowRay.direction));
                        }
                    }
                }
                return sky + light * sampler.numSamples;
            }
            return sky;
        }

        private Color RenderSkyColor(Ray ray)
        {
            if (environment == null)
                return Color.white;
            float fi = (float)Math.Atan2(ray.direction.x, ray.direction.z);
            float u = fi * 0.5f * (float)MathUtils.InvPi;
            float theta = (float)Math.Acos(ray.direction.y);

            float v = 1.0f - theta * (float)MathUtils.InvPi;

            return environment.Sample(u, v) * intensity;
        }

        /// <summary>
        /// Mie散射相位方程
        /// </summary>
        /// <param name="cosAngle">视线和光线的夹角</param>
        /// <returns></returns>
        private float Mie(float cosAngle)
        {
            return m_Param1 * (m_Param2 * (1.0f + cosAngle * cosAngle)) / (m_Param3 * (float)Math.Pow(m_Param4 - m_Param0 * cosAngle, 1.5f));
        }
    }
}