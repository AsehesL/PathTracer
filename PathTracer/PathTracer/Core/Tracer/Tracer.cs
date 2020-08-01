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

        public abstract Color FastTracing(Ray ray);

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

        public override Color FastTracing(Ray ray)
        {

            RayCastHit hit;
            hit.distance = double.MaxValue;

            if (sceneData.Raycast(ray, m_Epsilon, out hit))
            {
                if (hit.shader == null)
                {
                    float vdn = (float) Math.Max(0, Vector3.Dot(-1.0 * ray.direction, hit.normal));
                    return new Color(vdn, vdn, vdn, 1.0f);
                }
                else
                {
                    return hit.shader.FastRender(ray, hit);
                }
            }
            else
            {
                return new Color(0.192f, 0.3f, 0.4745f, 1.0f);
            }
        }
    }

    //public class PathTracerWithSun : PathTracer
    //{
    //    public PathTracerWithSun(int tracingTimes, double epsilon) : base(tracingTimes, epsilon) { }

    //    public override Color Tracing(Ray ray, bool isSunTracing, Sky sky, SamplerBase sampler, int depth = 0)
    //    {
    //        if (depth > tracingTimes)
    //            return Color.black;

    //        RayCastHit hit;
    //        hit.distance = double.MaxValue;

    //        if (!isSunTracing)
    //        {
    //            if (sceneData.Raycast(ray, m_Epsilon, out hit))
    //            {
    //                hit.depth = depth;

    //                Vector3 w = sky == null ? Vector3.down : - 1.0 * sky.sundirection;
    //                Vector3 u = Vector3.Cross(new Vector3(0.00424f, 1, 0.00764f), w);
    //                u.Normalize();
    //                Vector3 v = Vector3.Cross(u, w);
    //                Vector3 sp = SunLightOffset(sampler.Sample(), 0.2f);
    //                Vector3 l = sp.x * u + sp.y * v + sp.z * w;
    //                if (Vector3.Dot(l, hit.normal) < 0.0)
    //                    l = -sp.x * u - sp.y * v - sp.z * w;
    //                l.Normalize();
    //                Ray lray = new Ray(hit.hit, l);

    //                Color color = hit.shader == null ? new Color(1,0,1) : hit.shader.Render(this, false, sky, sampler, ray, hit, m_Epsilon);

    //                return color + Tracing(lray, true, sky, sampler, depth);
    //            }
    //            else
    //            {
    //                if (sky != null)
    //                    return sky.RenderColor(ray.direction, false);
    //                return Color.black;
    //            }
    //        }
    //        else
    //        {
    //            if (sceneData.Raycast(ray, m_Epsilon, out hit))
    //            {
    //                hit.depth = depth;

    //                return hit.shader == null ? new Color(1, 0, 1) : hit.shader.Render(this, true, sky, sampler, ray, hit, m_Epsilon);
    //            }
    //            else
    //            {
    //                if (sky != null)
    //                    return sky.RenderColor(ray.direction, true);
    //                return Color.black;
    //            }
    //        }
    //    }

    //    Vector3 SunLightOffset(Vector2 sample, float offset)
    //    {
    //        float a = offset * offset;

    //        double phi = 2.0f * Math.PI * sample.x;
    //        double cos_theta = Math.Sqrt((1.0 - sample.y) / (1.0 + (a * a - 1.0) * sample.y));
    //        double sin_theta = Math.Sqrt(1.0 - cos_theta * cos_theta);

    //        return new Vector3(Math.Cos(phi) * sin_theta, Math.Sin(phi) * sin_theta, cos_theta);
    //    }
    //}
}
