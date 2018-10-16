using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public abstract class Tracer
    {
        public int tracingTimes
        {
            get { return m_TracingTimes; }
        }

        public SceneData sceneData
        {
            get { return m_SceneData; }
            set { m_SceneData = value; }
        }

        private int m_TracingTimes;

        protected double m_Epsilon;

        protected SceneData m_SceneData;

        public Tracer(int tracingTimes, double epsilon)
        {
            m_TracingTimes = tracingTimes;
            m_Epsilon = epsilon;
        }

        public abstract Color Tracing(Ray ray, Sky sky, SamplerBase sampler, int depth = 0);

        public Color FastTracing(Ray ray)
        {
            RayCastHit hit;
            hit.distance = double.MaxValue;

            if (sceneData.Raycast(ray, m_Epsilon, out hit))
            {
                float vdn = (float)Math.Max(0, Vector3.Dot(-1.0 * ray.direction, hit.normal));
                return new Color(vdn, vdn, vdn, 1.0f);
            }
            else
            {
                return new Color(0.192f, 0.3f, 0.4745f, 1.0f);
            }
        }
    }

    public class PathTracer : Tracer
    {
        public PathTracer(int tracingTimes, double epsilon) : base(tracingTimes, epsilon) { }

        public override Color Tracing(Ray ray, Sky sky, SamplerBase sampler, int depth = 0)
        {
            if (depth > tracingTimes)
                return Color.black;

            RayCastHit hit;
            hit.distance = double.MaxValue;

            if (sceneData.Raycast(ray, m_Epsilon, out hit))
            {
                hit.depth = depth;
                if (hit.shader == null)
                    return new Color(1, 0, 1);
                return hit.shader.Render(this, sky, sampler, ray, hit, m_Epsilon);
            }
            else
            {
                if (sky != null)
                    return sky.RenderColor(ray.direction);
                return Color.black;
            }
        }
    }
}
