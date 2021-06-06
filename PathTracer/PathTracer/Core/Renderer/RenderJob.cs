using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ASL.PathTracer
{

    public delegate void RenderJobCallBackDelegate(int progress, int total);

    public interface IRenderJobResult
    {
        bool ApplyToRenderResult(IRenderResult result);
    }

    public interface IRenderWork<T, W> where T : IRenderJobResult where W : RendererBase
    {
        bool IsRenderComplete();

        T Render(Scene scene, W renderer, SamplerBase sampler);
    }

    /// <summary>
    /// 多线程渲染Job
    /// </summary>
    public class RenderJob<T, W> where T : IRenderJobResult where W : RendererBase
    {

        private class Job
        {
            private ConcurrentQueue<IRenderWork<T, W>> m_Works;
            private ConcurrentQueue<T> m_Results;
            private ManualResetEvent m_ResetEvent;

            private SamplerBase m_Sampler;

            private Scene m_Scene;
            private W m_Renderer;

            public Job(RenderConfig config, Scene scene, W renderer, ConcurrentQueue<IRenderWork<T, W>> works, ConcurrentQueue<T> results, ManualResetEvent resetEvent)
            {
                m_Works = works;
                m_Results = results;
                m_ResetEvent = resetEvent;

                m_Scene = scene;
                m_Renderer = renderer;

                m_Sampler = SamplerFactory.Create(config.samplerType, config.numSamples, config.numSets);
            }

            public void Render()
            {
                while (m_Works.Count > 0)
                {
                    IRenderWork<T, W> work = null;
                    if (!m_Works.TryDequeue(out work))
                        break;
                    if (work == null)
                        continue;
                    while(!work.IsRenderComplete())
                    {
                        T result = work.Render(m_Scene, m_Renderer, m_Sampler);
                        m_Results.Enqueue(result);
                    }

                    m_ResetEvent.Set();
                }
            }
        }

        private ConcurrentQueue<IRenderWork<T, W>> m_Works;
        private ConcurrentQueue<T> m_Results;
        private ManualResetEvent m_ResetEvent;
        private Job[] m_Jobs;

        public RenderJob(RenderConfig config, Scene scene, W renderer)
        {
            m_Works = new ConcurrentQueue<IRenderWork<T, W>>();
            m_Results = new ConcurrentQueue<T>();
            m_Jobs = new Job[Environment.ProcessorCount];
            m_ResetEvent = new ManualResetEvent(false);

            for (int i = 0; i < m_Jobs.Length; i++)
            {
                m_Jobs[i] = new Job(config, scene, renderer, m_Works, m_Results, m_ResetEvent);
            }
        }

        public void AddWork(IRenderWork<T, W> work)
        {
            m_Works.Enqueue(work);
        }

        public void Render(IRenderResult renderResult, RenderJobCallBackDelegate progressCallBackAction = null)
        {
            int total = m_Works.Count;

            Task[] tasks = new Task[m_Jobs.Length];
            for (int i = 0; i < m_Jobs.Length; i++)
            {
                tasks[i] = Task.Run(new Action(m_Jobs[i].Render));
            }

            while (m_Works.Count > 0)
            {
                m_ResetEvent.WaitOne();
                m_ResetEvent.Reset();
                //if (m_Results.Count > 0)
                //{
                //    T result = default(T);
                //    if (!m_Results.TryDequeue(out result))
                //        break;
                //    progressCallBackAction?.Invoke(total - m_Works.Count, total, result);
                //}
                progressCallBackAction?.Invoke(total - m_Works.Count, total);
            }

            //等待所有任务结束
            Task.WaitAll(tasks);

            if (renderResult != null)
            {
                while (m_Results.Count > 0)
                {
                    T result = default(T);
                    if (!m_Results.TryDequeue(out result))
                        return;
                    if (result != null)
                        result.ApplyToRenderResult(renderResult);
                }
            }

        }
    }
}
