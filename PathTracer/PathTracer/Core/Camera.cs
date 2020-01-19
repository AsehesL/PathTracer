#define USE_MULTI_THREAD

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASL.PathTracer
{
    public class Camera
    {
        public Vector3 position { get; private set; }

        public Vector3 right
        {
            get { return m_Right; }
        }

        public Vector3 up
        {
            get { return m_Up; }
        }

        public Vector3 forward
        {
            get { return m_Forward; }
        }

        public double near { get; private set; }

        public double fieldOfView { get; private set; }

        private Vector3 m_Right;
        private Vector3 m_Up;
        private Vector3 m_Forward;

        private Texture m_RenderTarget;

        private double m_Height;
        private double m_Width;

        private SamplerType m_SamplerType;
        private int m_NumSamples;
        private int m_NumSets;

        public Camera(Vector3 position, Vector3 euler, double near, double fieldOfView)
        {
            this.near = near;
            this.fieldOfView = fieldOfView;
            this.position = position;

            double cosx = Math.Cos(euler.x * 0.01745329252 * 0.5);
            double cosy = Math.Cos(euler.y * 0.01745329252 * 0.5);
            double cosz = Math.Cos(euler.z * 0.01745329252 * 0.5);

            double sinx = Math.Sin(euler.x * 0.01745329252 * 0.5);
            double siny = Math.Sin(euler.y * 0.01745329252 * 0.5);
            double sinz = Math.Sin(euler.z * 0.01745329252 * 0.5);

            double rx = cosy * sinx * cosz + siny * cosx * sinz;
            double ry = siny * cosx * cosz - cosy * sinx * sinz;
            double rz = cosy * cosx * sinz - siny * sinx * cosz;
            double rw = cosy * cosx * cosz + siny * sinx * sinz;

            double x2 = 2.0 * rx * rx;
            double y2 = 2.0 * ry * ry;
            double z2 = 2.0 * rz * rz;
            double xy = 2.0 * rx * ry;
            double xz = 2.0 * rx * rz;
            double xw = 2.0 * rx * rw;
            double yz = 2.0 * ry * rz;
            double yw = 2.0 * ry * rw;
            double zw = 2.0 * rz * rw;

            double ra = 1.0 - y2 - z2;
            double rb = xy + zw;
            double rc = xz - yw;
            double rd = xy - zw;
            double re = 1.0 - x2 - z2;
            double rf = yz + xw;
            double rg = xz + yw;
            double rh = yz - xw;
            double ri = 1.0 - x2 - y2;

            m_Right.x = ra;
            m_Right.y = rb;
            m_Right.z = rc;

            m_Up.x = rd;
            m_Up.y = re;
            m_Up.z = rf;

            m_Forward.x = rg;
            m_Forward.y = rh;
            m_Forward.z = ri;

        }

        public void SetRenderTarget(Texture renderTarget)
        {
            m_RenderTarget = renderTarget;
            if (m_RenderTarget == null)
                return;

            float aspect = (((float) renderTarget.width) / renderTarget.height);

            m_Height = near * Math.Tan(fieldOfView * 0.5 * 0.01745329252);
            m_Width = aspect * m_Height;
        }

        public void SetSampler(SamplerType samplerType, int numSamples, int numSets = 83)
        {
            m_SamplerType = samplerType;
            m_NumSamples = numSamples;
            m_NumSets = numSets;
            //m_Samplers = new SamplerBase[Environment.ProcessorCount];
            //for (int i = 0; i < m_Samplers.Length; i++)
            //    m_Samplers[i] = SamplerFactory.Create(samplerType, numSamples, numSets);
            //m_Sampler = sampler;
        }

        public Ray GetRayFromPixel(double x, double y)
        {
            if (m_RenderTarget == null)
                throw new System.NullReferenceException();
            x = (m_RenderTarget.width - 1 - x) / m_RenderTarget.width * 2 - 1;
            y = y / m_RenderTarget.height * 2 - 1;

            Vector2 point = new Vector2(x * m_Width, y * m_Height);
            return GetRayFromPoint(point);
        }

        public Ray GetRayFromPoint(Vector2 point)
        {
            Vector3 dir = m_Right * point.x + m_Up * point.y + m_Forward * this.near;
            Vector3 ori = this.position + dir;
            dir.Normalize();
            return new Ray(ori, dir);
        }

        public void Render(Scene scene, bool fastRender, System.Action<int, int> progressCallBackAction = null)
        {
            if (scene == null)
                throw new System.ArgumentNullException();
            if (m_RenderTarget == null)
                throw new System.NullReferenceException("未设置RenderTarget");

            RenderJob job = new RenderJob(m_SamplerType, m_NumSamples, m_NumSets, m_RenderTarget.width, m_RenderTarget.height, scene, this, fastRender);

            for (int j = 0; j < m_RenderTarget.height; j += 32)
            {
                for (int i = 0; i < m_RenderTarget.width; i += 32)
                {
                    job.AddTile(i, j);
                }
            }
            job.Render(m_RenderTarget, progressCallBackAction);

        }

        internal void RenderPixel(int x, int y, SamplerBase sampler, Scene scene)
        {
            Color col = Color.black;
            for (int k = 0; k < sampler.numSamples; k++)
            {
                var sample = sampler.Sample();
                Ray ray = GetRayFromPixel(x + sample.x, y + sample.y);
                col += scene.tracer.Tracing(ray, scene.sky, sampler);
            }

            col /= sampler.numSamples;
            col.a = 1.0f;
            m_RenderTarget.SetPixel(x, y, col);
        }

        internal Color RenderPixelToColor(int x, int y, SamplerBase sampler, Scene scene)
        {
            Color col = Color.black;
            for (int k = 0; k < sampler.numSamples; k++)
            {
                var sample = sampler.Sample();
                Ray ray = GetRayFromPixel(x + sample.x, y + sample.y);
                var pix = scene.tracer.Tracing(ray, scene.sky, sampler);
                col += pix;
                //m_RenderTarget.SetPixel(x, y, scene.tracer.Tracing(ray, scene.sky, sampler));
            }

            col /= sampler.numSamples;
            col.a = 1.0f;
            return col;
        }

        internal Color FastRenderPixelToColor(int x, int y, Scene scene)
        {
            Ray ray = GetRayFromPixel(x + 0.5f, y + 0.5f);
            return scene.tracer.FastTracing(ray);
        }

        private void WaitRender(List<ManualResetEvent> events)
        {

            int finished = WaitHandle.WaitAny(events.ToArray());
            if (finished == WaitHandle.WaitTimeout)
            {
            }

            events.RemoveAt(finished);
        }

        class RenderJob
        {
            private struct Tile
            {
                public int x;
                public int y;
            }

            private struct Result
            {
                public int x;
                public int y;
                public Color color;
            }

            private class Job
            {
                public SamplerBase sampler;
                public Camera camera;
                public Scene scene;

                public bool isFastRender;

                private ConcurrentQueue<Tile> m_Tiles;
                private ConcurrentQueue<Result> m_Results;
                private ManualResetEvent m_ResetEvent;

                public uint renderWidth;
                public uint renderHeight;

                public Job(ConcurrentQueue<Tile> tiles, ConcurrentQueue<Result> results, ManualResetEvent resetEvent)
                {
                    m_Tiles = tiles;
                    m_Results = results;
                    m_ResetEvent = resetEvent;
                }

                public void Render()
                {
                    while (m_Tiles.Count > 0)
                    {
                        Tile tile;
                        if (!m_Tiles.TryDequeue(out tile))
                            break;
                        for (int j = tile.y; j < renderHeight && j - tile.y < 32; j++)
                        {
                            for (int i = tile.x; i < renderWidth && i - tile.x < 32; i++)
                            {
                                Color col;
                                if (isFastRender)
                                {
                                    col = camera.FastRenderPixelToColor(i, j, scene);
                                }
                                else
                                {
                                    col = camera.RenderPixelToColor(i, j, sampler, scene);
                                }

                                m_Results.Enqueue(new Result() { color = col, x = i, y = j });

                            }
                        }

                        m_ResetEvent.Set();
                    }
                }
            }

            private ConcurrentQueue<Tile> m_Tiles;
            private ConcurrentQueue<Result> m_Results;
            private ManualResetEvent m_ResetEvent;
            private Job[] m_Jobs;
            //private TempTexture m_TempTexture;

            public RenderJob(SamplerType samplerType, int numSamples, int numSets, uint renderWidth, uint renderHeight, Scene scene, Camera camera, bool fastRender = false)
            {
                m_Tiles = new ConcurrentQueue<Tile>();
                m_Results = new ConcurrentQueue<Result>();
                m_Jobs = new Job[Environment.ProcessorCount];
                m_ResetEvent = new ManualResetEvent(false);

                for (int i = 0; i < m_Jobs.Length; i++)
                {
                    m_Jobs[i] = new Job(m_Tiles, m_Results, m_ResetEvent);
                    m_Jobs[i].camera = camera;
                    m_Jobs[i].scene = scene;
                    m_Jobs[i].sampler = SamplerFactory.Create(samplerType, numSamples, numSets);
                    m_Jobs[i].isFastRender = fastRender;
                    m_Jobs[i].renderWidth = renderWidth;
                    m_Jobs[i].renderHeight = renderHeight;
                }
            }

            public void AddTile(int x, int y)
            {
                m_Tiles.Enqueue(new Tile { x = x, y = y });
            }

            public void Render(Texture texture, System.Action<int, int> progressCallBackAction = null)
            {
                int total = m_Tiles.Count;

                Task[] tasks = new Task[m_Jobs.Length];
                for (int i = 0; i < m_Jobs.Length; i++)
                {
                    tasks[i] = Task.Run(new Action(m_Jobs[i].Render));
                }

                while (m_Tiles.Count > 0)
                {
                    m_ResetEvent.WaitOne();
                    m_ResetEvent.Reset();
                    progressCallBackAction?.Invoke(total - m_Tiles.Count, total);
                }

                Task.WaitAll(tasks);

                ResultToTexture(texture);
            }

            private void ResultToTexture(Texture texture)
            {
                if (texture == null)
                    return;
                while (m_Results.Count > 0)
                {
                    Result result;
                    if (!m_Results.TryDequeue(out result))
                        return;
                    texture.SetPixel(result.x, result.y, result.color);
                }
            }
        }
    }
}