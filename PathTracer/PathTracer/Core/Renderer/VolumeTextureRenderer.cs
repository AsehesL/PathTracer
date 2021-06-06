using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ASL.PathTracer
{
    //public class SHVolumeTexture : IRenderResult
    //{
    //    private VolumeTexture m_VolumeTexture0;
    //    private VolumeTexture m_VolumeTexture1;
    //    private VolumeTexture m_VolumeTexture2;

    //    private Bounds m_Bounds;

    //    public SHVolumeTexture(Bounds bounds, uint width, uint height, uint depth)
    //    {
    //        m_Bounds = bounds;
    //        m_VolumeTexture0 = new VolumeTexture(width, height, depth);
    //        m_VolumeTexture1 = new VolumeTexture(width, height, depth);
    //        m_VolumeTexture2 = new VolumeTexture(width, height, depth);
    //    }

    //    public void SetVoxel(int x, int y, int z, Color shColor0, Color shColor1, Color shColor2, Color shColor3)
    //    {
    //        m_VolumeTexture0.SetVoxel(x, y, z, new Color(shColor0.r, shColor1.r, shColor2.r, shColor3.r));
    //        m_VolumeTexture1.SetVoxel(x, y, z, new Color(shColor0.g, shColor1.g, shColor2.g, shColor3.g));
    //        m_VolumeTexture2.SetVoxel(x, y, z, new Color(shColor0.b, shColor1.b, shColor2.b, shColor3.b));
    //    }

    //    public string GetExtensions()
    //    {
    //        return "SHVolumeTexture文件|*.vxtex";
    //    }

    //    public bool Save(string path)
    //    {
    //        if (string.IsNullOrEmpty(path))
    //            return false;
    //        System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
    //        System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs);

    //        float centerX = (float)m_Bounds.center.x;
    //        float centerY = (float)m_Bounds.center.y;
    //        float centerZ = (float)m_Bounds.center.z;

    //        float sizeX = (float)m_Bounds.size.x;
    //        float sizeY = (float)m_Bounds.size.y;
    //        float sizeZ = (float)m_Bounds.size.z;

    //        int width = (int)m_VolumeTexture0.width;
    //        int height = (int)m_VolumeTexture0.height;
    //        int depth = (int)m_VolumeTexture0.depth;

    //        bw.Write(centerX);
    //        bw.Write(centerY);
    //        bw.Write(centerZ);

    //        bw.Write(sizeX);
    //        bw.Write(sizeY);
    //        bw.Write(sizeZ);

    //        bw.Write(width);
    //        bw.Write(height);
    //        bw.Write(depth);

    //        for(int i=0;i<width;i++)
    //        {
    //            for(int j=0;j<height;j++)
    //            {
    //                for(int k=0;k<depth;k++)
    //                {
    //                    Color sh0 = m_VolumeTexture0.GetVoxel(i, j, k);
    //                    Color sh1 = m_VolumeTexture1.GetVoxel(i, j, k);
    //                    Color sh2 = m_VolumeTexture2.GetVoxel(i, j, k);

    //                    bw.Write(sh0.r); bw.Write(sh0.g); bw.Write(sh0.b); bw.Write(sh0.a);
    //                    bw.Write(sh1.r); bw.Write(sh1.g); bw.Write(sh1.b); bw.Write(sh1.a);
    //                    bw.Write(sh2.r); bw.Write(sh2.g); bw.Write(sh2.b); bw.Write(sh2.a);
    //                }
    //            }
    //        }

    //        bw.Close();
    //        fs.Close();
    //        return true;
    //    }
    //}


    //class VolumeTextureRenderJob
    //{
    //    private struct Probe
    //    {
    //        public int x;
    //        public int y;
    //        public int z;
    //        public Vector3 position;
    //    }

    //    private struct Voxel
    //    {
    //        public int x;
    //        public int y;
    //        public int z;
    //        public Color shColor0;
    //        public Color shColor1;
    //        public Color shColor2;
    //        public Color shColor3;
    //    }

    //    private class Job
    //    {
    //        public SamplerBase sampler;
    //        public Scene scene;
    //        public VolumeTextureRenderer renderer;

    //        private ConcurrentQueue<Probe> m_Probes;
    //        private ConcurrentQueue<Voxel> m_Results;
    //        private ManualResetEvent m_ResetEvent;

    //        private RenderState m_RenderState;

    //        public Job(ConcurrentQueue<Probe> probes, ConcurrentQueue<Voxel> results, ManualResetEvent resetEvent)
    //        {
    //            m_Probes = probes;
    //            m_Results = results;
    //            m_ResetEvent = resetEvent;
    //            m_RenderState = new RenderState();
    //        }

    //        public void Render()
    //        {
    //            while (m_Probes.Count > 0)
    //            {
    //                Probe probe;
    //                if (!m_Probes.TryDequeue(out probe))
    //                    break;

    //                sampler.ResetSampler(); //重置采样器状态
    //                m_RenderState.ResetState(); //重置渲染状态
    //                Color shcol0 = Color.black;
    //                Color shcol1 = Color.black;
    //                Color shcol2 = Color.black;
    //                Color shcol3 = Color.black;
    //                double weight = 4.0 * Math.PI / sampler.numSamples;
    //                while (sampler.NextSample())
    //                {
    //                    Vector3 dir = sampler.SampleSphere();
    //                    Ray ray = new Ray(probe.position, dir);
    //                    Color col = renderer.Tracing(ray, sampler, m_RenderState);
    //                    col.FixColor();

    //                    shcol0 += col * (float)(R_Sphere0(dir) * weight);
    //                    shcol1 += col * (float)(R_Sphere1_0(dir) * weight);
    //                    shcol2 += col * (float)(R_Sphere1_1(dir) * weight);
    //                    shcol3 += col * (float)(R_Sphere1_2(dir) * weight);
    //                }

    //                m_Results.Enqueue(new Voxel() { shColor0 = shcol0, shColor1 = shcol1, shColor2 = shcol2, shColor3 = shcol3, x = probe.x, y = probe.y, z = probe.z });

    //                m_ResetEvent.Set();
    //            }
    //        }

    //        private double R_Sphere0(Vector3 dir)
    //        {
    //            return 0.5 * Math.Sqrt(0.31830988619);
    //        }

    //        private double R_Sphere1_0(Vector3 dir)
    //        {
    //            return 0.5 * Math.Sqrt(3 * 0.31830988619) * dir.y;
    //        }

    //        private double R_Sphere1_1(Vector3 dir)
    //        {
    //            return 0.5 * Math.Sqrt(3 * 0.31830988619) * dir.z;
    //        }

    //        private double R_Sphere1_2(Vector3 dir)
    //        {
    //            return 0.5 * Math.Sqrt(3 * 0.31830988619) * dir.x;
    //        }
    //    }

    //    private ConcurrentQueue<Probe> m_Probes;
    //    private ConcurrentQueue<Voxel> m_Results;
    //    private ManualResetEvent m_ResetEvent;
    //    private Job[] m_Jobs;

    //    public VolumeTextureRenderJob(RenderConfig config, Scene scene, VolumeTextureRenderer volumeTextureRenderer)
    //    {
    //        m_Probes = new ConcurrentQueue<Probe>();
    //        m_Results = new ConcurrentQueue<Voxel>();
    //        m_Jobs = new Job[Environment.ProcessorCount];
    //        m_ResetEvent = new ManualResetEvent(false);

    //        for (int i = 0; i < m_Jobs.Length; i++)
    //        {
    //            m_Jobs[i] = new Job(m_Probes, m_Results, m_ResetEvent);
    //            m_Jobs[i].renderer = volumeTextureRenderer;
    //            m_Jobs[i].scene = scene;
    //            m_Jobs[i].sampler = SamplerFactory.Create(config.samplerType, config.numSamples, config.numSets);
    //        }
    //    }

    //    public void AddProbe(int x, int y, int z, Vector3 position)
    //    {
    //        m_Probes.Enqueue(new Probe { x = x, y = y, z = z, position = position });
    //    }

    //    public void Render(SHVolumeTexture texture, System.Action<int, int> progressCallBackAction = null)
    //    {
    //        int total = m_Probes.Count;

    //        Task[] tasks = new Task[m_Jobs.Length];
    //        for (int i = 0; i < m_Jobs.Length; i++)
    //        {
    //            tasks[i] = Task.Run(new Action(m_Jobs[i].Render));
    //        }

    //        while (m_Probes.Count > 0)
    //        {
    //            m_ResetEvent.WaitOne();
    //            m_ResetEvent.Reset();
    //            progressCallBackAction?.Invoke(total - m_Probes.Count, total);
    //        }

    //        //等待所有任务结束
    //        Task.WaitAll(tasks);

    //        ResultToTexture(texture);
    //    }

    //    private void ResultToTexture(SHVolumeTexture texture)
    //    {
    //        if (texture == null)
    //            return;
    //        while (m_Results.Count > 0)
    //        {
    //            Voxel result;
    //            if (!m_Results.TryDequeue(out result))
    //                return;
    //            texture.SetVoxel(result.x, result.y, result.z, result.shColor0, result.shColor1, result.shColor2, result.shColor3);
    //        }
    //    }
    //}

    //public class VolumeTextureRenderer : Tracer
    //{
    //    public VolumeTextureRenderer(Scene scene) : base(scene)
    //    { }

    //    protected override IRenderResult OnRender(RenderConfig config, Action<int, int> progressCallBackAction)
    //    {
    //        Bounds bounds = sceneData.GetBounds();
    //        bounds = new Bounds(bounds.center, bounds.size * 0.93f);

    //        SHVolumeTexture result = new SHVolumeTexture(bounds, config.width, config.width, config.height);

    //        VolumeTextureRenderJob job = new VolumeTextureRenderJob(config, m_Scene, this);

    //        for (int i = 0;i<config.width;i++)
    //        {
    //            for (int j = 0; j < config.width; j++)
    //            {
    //                for (int k = 0; k < config.height; k++)
    //                {
    //                    double x = bounds.min.x + (bounds.max.x - bounds.min.x) * ((double)i / (config.width - 1));
    //                    double z = bounds.min.z + (bounds.max.z - bounds.min.z) * ((double)j / (config.width - 1));
    //                    double y = bounds.min.y + (bounds.max.y - bounds.min.y) * ((double)k / (config.height - 1));
    //                    job.AddProbe(i, j, k, new Vector3(x, y, z));
    //                }
    //            }
    //        }

    //        job.Render(result, progressCallBackAction);

    //        return result;
    //    }

    //    protected override IRenderResult OnRenderDebugSinglePixel(int x, int y, RenderConfig config)
    //    {
    //        return null;
    //    }

    //    public override Color Tracing(Ray ray, SamplerBase sampler, RenderState renderState, int depth = 0)
    //    {
    //        if (depth > tracingTimes)
    //        {
    //            return Color.black;
    //        }

    //        RayCastHit hit;
    //        hit.distance = double.MaxValue;

    //        Color color = default(Color);

    //        bool isHit = false;
    //        if (sceneData.Raycast(ray, out hit))
    //        {
    //            hit.depth = depth;
    //               if (hit.shader == null)
    //                    color += new Color(1, 0, 1);
    //                else
    //                    color += hit.shader.Render(this, sampler, renderState, ray, hit);

    //            isHit = true;
    //        }

    //        if (!isHit)
    //        {
    //            if (sceneData.sky != null)
    //                color += sceneData.sky.RenderColor(this, sampler, renderState, ray, hit, depth, isHit);
    //            else
    //                color += Color.black;
    //        }

    //        return color;
    //    }
    //}
}
