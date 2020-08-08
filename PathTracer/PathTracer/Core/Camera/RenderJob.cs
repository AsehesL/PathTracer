using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ASL.PathTracer
{
    /// <summary>
    /// 渲染状态对象
    /// </summary>
    public interface IRenderStateObject
    {
        void ResetState();
    }

    /// <summary>
    /// 为不同RenderJob存放shader渲染的临时数据，每个RenderJob会维护一个RenderState
    /// </summary>
    public class RenderState
    {
        private Dictionary<System.Type, IRenderStateObject> m_RenderStateObjects;

        /// <summary>
        /// 获取渲染状态对象
        /// </summary>
        /// <typeparam name="T">渲染状态对象类型</typeparam>
        /// <param name="type">目标类型</param>
        /// <returns></returns>
        public T GetRenderStateObject<T>(System.Type type) where T : IRenderStateObject
        {
            if (m_RenderStateObjects == null)
                m_RenderStateObjects = new Dictionary<Type, IRenderStateObject>();
            if (m_RenderStateObjects.ContainsKey(type) == false)
            {
                var state = (IRenderStateObject)System.Activator.CreateInstance(typeof(T));
                state.ResetState();
                m_RenderStateObjects[type] = state;
            }
            return (T)m_RenderStateObjects[type];
        }

        /// <summary>
        /// 重置所有渲染状态
        /// </summary>
        public void ResetState()
        {
            if (m_RenderStateObjects != null)
            {
                foreach (var state in m_RenderStateObjects)
                {
                    if (state.Value != null)
                        state.Value.ResetState();
                }
            }
        }
    }

    /// <summary>
    /// 多线程渲染Job
    /// </summary>
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

            private ConcurrentQueue<Tile> m_Tiles;
            private ConcurrentQueue<Result> m_Results;
            private ManualResetEvent m_ResetEvent;

            private RenderState m_RenderState;

            public uint renderWidth;
            public uint renderHeight;

            public Job(ConcurrentQueue<Tile> tiles, ConcurrentQueue<Result> results, ManualResetEvent resetEvent)
            {
                m_Tiles = tiles;
                m_Results = results;
                m_ResetEvent = resetEvent;
                m_RenderState = new RenderState();
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
                            Color col = camera.RenderPixelToColor(i, j, sampler, m_RenderState, scene);

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

        public RenderJob(SamplerType samplerType, int numSamples, int numSets, uint renderWidth, uint renderHeight, Scene scene, CameraBase camera)
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

            //等待所有任务结束
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
