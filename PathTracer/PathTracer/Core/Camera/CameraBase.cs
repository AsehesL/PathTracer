#define USE_MULTI_THREAD

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ASL.PathTracer
{
    [Flags]
    public enum PTBufferType
    {
        None = 0,
        Color = 1,
        Volumetric = 2,
    }

    public abstract class CameraBase
    {
        public Vector3 position { get; protected set; }

        public Vector3 right { get; private set; }

        public Vector3 up { get; private set; }

        public Vector3 forward { get; private set; }

        protected Texture m_RenderTarget;

        private SamplerType m_SamplerType;
        private int m_NumSamples;
        private int m_NumSets;

        public CameraBase(Vector3 position, Vector3 euler)
        {
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

            this.right = new Vector3(ra, rb, rc);
            this.up = new Vector3(rd, re, rf);
            this.forward = new Vector3(rg, rh, ri);
        }

        public void SetRenderTarget(Texture renderTarget)
        {
            m_RenderTarget = renderTarget;
            if (m_RenderTarget == null)
                return;

            ModifyResolution(renderTarget.width, renderTarget.height);
        }

        protected virtual void ModifyResolution(uint width, uint height) { }

        public void SetSampler(SamplerType samplerType, int numSamples, int numSets = 83)
        {
            m_SamplerType = samplerType;
            m_NumSamples = numSamples;
            m_NumSets = numSets;
        }

        public abstract Ray GetRay(int x, int y, SamplerBase sampler);

        public abstract Ray GetRayWithoutSampler(float x, float y);

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
                Ray ray = GetRay(x, y, sampler);
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
                Ray ray = GetRay(x, y, sampler);
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
            Ray ray = GetRayWithoutSampler(x + 0.5f, y + 0.5f);
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
                public CameraBase camera;
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

            public RenderJob(SamplerType samplerType, int numSamples, int numSets, uint renderWidth, uint renderHeight, Scene scene, CameraBase camera, bool fastRender = false)
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
