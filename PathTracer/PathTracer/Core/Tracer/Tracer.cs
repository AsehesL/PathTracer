using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    /// <summary>
    /// 光线追踪器基类
    /// </summary>
    public abstract class Tracer
    {
#if DEBUG
        public bool isDebugging;
#endif

        public SceneData sceneData;

        public RenderChannel renderChannel;

        public int tracingTimes;

        public double epsilon { get { return m_Epsilon; } }

        protected double m_Epsilon;

        public Tracer(double epsilon)
        {
            m_Epsilon = epsilon;
        }

        public abstract Color Tracing(Ray ray, SamplerBase sampler, RenderState renderState, int depth = 0);

        //public abstract Color PreviewTracing(Ray ray, RenderChannel renderChannel);

        public bool TracingOnce(Ray ray)
        {
            RayCastHit hit;
            hit.distance = double.MaxValue;

            if (sceneData.Raycast(ray, m_Epsilon, out hit))
            {
                return true;
            }
            return false;
        }

        public bool TracingOnce(Ray ray, out RayCastHit hit)
        {
            hit.distance = double.MaxValue;
            if (sceneData.Raycast(ray, m_Epsilon, out hit))
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 路径追踪器
    /// </summary>
    public class PathTracer : Tracer
    {
        public PathTracer(double epsilon) : base(epsilon) { }

        public override Color Tracing(Ray ray, SamplerBase sampler, RenderState renderState, int depth = 0)
        {
            if (renderChannel != RenderChannel.Full && renderChannel != RenderChannel.SkyLight)
            {
#if DEBUG
                Color previewcolor = PreviewTracing(ray, sampler, renderChannel);
                if (isDebugging)
                {
                    Log.AddLog(LogType.Debugging, $"深度：{depth}，射线：{ray}，颜色：{previewcolor}");
                }
                return previewcolor;
#else
                return PreviewTracing(ray, sampler, renderChannel);
#endif
            }

            if (depth > tracingTimes)
            {
#if DEBUG
                if (isDebugging)
                {
                    Log.AddLog(LogType.Debugging, $"深度：{depth}，射线：{ray}，颜色：{Color.black}");
                }
#endif
                return Color.black;
            }

            RayCastHit hit;
            hit.distance = double.MaxValue;
            hit.depth = depth;

            Color color = default(Color);
            //Color volumetric = default(Color);

            bool isHit = false;
            if (sceneData.Raycast(ray, m_Epsilon, out hit))
            {

                if (renderChannel == RenderChannel.Full)
                {
                    if (hit.shader == null)
                        color += new Color(1, 0, 1);
                    else
                        color += hit.shader.Render(this, sampler, renderState, ray, hit);
                }

                isHit = true;
            }

            if (!isHit || (isHit && depth == 0 && sceneData.sky != null && sceneData.sky.shouldRenderParticipatingMedia))
            {
                if (sceneData.sky != null)
                    color += sceneData.sky.RenderColor(this, sampler, renderState, ray, hit, depth, isHit);
                else
                    color += Color.black;
            }


            if (depth == 0)
                color /= sampler.numSamples;

            //return color + volumetric;
#if DEBUG
            if (isDebugging)
            {
                Log.AddLog(LogType.Debugging, $"深度：{depth}，射线：{ray}，颜色：{color}");
            }
#endif
            return color;
        }

        private Color PreviewTracing(Ray ray, SamplerBase sampler, RenderChannel renderChannel)
        {
            RayCastHit hit;
            hit.distance = double.MaxValue;

            if (sceneData.Raycast(ray, m_Epsilon, out hit))
            {
                if (hit.shader == null)
                {
                    float vdn = (float)Math.Max(0, Vector3.Dot(-1.0 * ray.direction, hit.normal));
                    return new Color(vdn, vdn, vdn, 1.0f);
                }
                else
                {
                    return hit.shader.RenderPreviewChannel(this, sampler, ray, hit, renderChannel);
                }
            }
            else
            {
                return new Color(0.192f, 0.3f, 0.4745f, 1.0f);
            }
        }
    }
}
