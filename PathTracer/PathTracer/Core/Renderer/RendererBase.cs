using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    /// <summary>
    /// 渲染通道
    /// </summary>
    public enum RenderChannel
    {
        Full,
        SkyLightOnly,
        DirectLightOnly,
        Albedo,
        Roughness,
        Metallic,
        WorldNormal,
        Occlusion,
        Emissive,
        Opacity,
        DiffuseNoLighting,
        Diffuse,
        DirectionalLightShadow,
        IndirectLighting,
    }

    public struct RenderConfig
    {
        public int traceTimes;
        public uint width;
        public uint height;
        public SamplerType samplerType;
        public int numSamples;
        public int numSets;
    }

    /// <summary>
    /// 渲染器基类
    /// </summary>
    public abstract class RendererBase
    {
#if DEBUG
        public bool isDebugging;
#endif

        public int tracingTimes;

        public Scene scene
        {
            get
            {
                if (m_Scene == null)
                    return null;
                return m_Scene;
            }
        }

        protected Scene m_Scene;

        public RendererBase(Scene scene)
        {
            m_Scene = scene;
        }

        public IRenderResult Render(RenderConfig config, RenderJobCallBackDelegate progressCallBackAction = null)
        {
            if (m_Scene == null)
                throw new System.ArgumentNullException();

            this.tracingTimes = config.traceTimes;
#if DEBUG
            this.isDebugging = false;
#endif
            try
            {
                return OnRender(config, progressCallBackAction);
            }
            catch(System.Exception e)
            {
                Log.Err(e.Message);
                Log.Err(e.StackTrace);
            }
            return null;
        }

#if DEBUG
        public IRenderResult RenderDebugSinglePixel(int x, int y, RenderConfig config)
        {
            if (m_Scene == null)
                throw new System.ArgumentNullException();
            
            this.tracingTimes = config.traceTimes;
            this.isDebugging = true;
           
            try
            {
                return OnRenderDebugSinglePixel(x, y, config);
            }
            catch (System.Exception e)
            {
                Log.Err(e.Message);
                Log.Err(e.StackTrace);
            }

            return null;
        }
#endif

        protected abstract IRenderResult OnRender(RenderConfig config, RenderJobCallBackDelegate progressCallBackAction);

#if DEBUG
        protected abstract IRenderResult OnRenderDebugSinglePixel(int x, int y, RenderConfig config);
#endif

        //public abstract Color Tracing(Ray ray, SamplerBase sampler, RenderState renderState, int depth = 0);

        ////public abstract Color PreviewTracing(Ray ray, RenderChannel renderChannel);

        //public bool TracingOnce(Ray ray)
        //{
        //    RayCastHit hit;
        //    hit.distance = double.MaxValue;

        //    if (sceneData.Raycast(ray, out hit))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //public bool TracingOnce(Ray ray, out RayCastHit hit)
        //{
        //    hit.distance = double.MaxValue;
        //    if (sceneData.Raycast(ray, out hit))
        //    {
        //        return true;
        //    }
        //    return false;
        //}
    }

}
