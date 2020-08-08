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
        Albedo,
        Roughness,
        Metallic,
        WorldNormal,
        Occlusion,
        Emissive,
        DiffuseNoLighting,
        Diffuse,
        DirectionalLightShadow,
    }

    /// <summary>
    /// 相机基类
    /// </summary>
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

        /// <summary>
        /// 执行相机渲染
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="renderChannel"></param>
        /// <param name="progressCallBackAction">渲染进度回调</param>
        public void Render(Scene scene, RenderChannel renderChannel, System.Action<int, int> progressCallBackAction = null)
        {
            if (scene == null)
                throw new System.ArgumentNullException();
            if (m_RenderTarget == null)
                throw new System.NullReferenceException("未设置RenderTarget");

            RenderJob job = new RenderJob(m_SamplerType, m_NumSamples, m_NumSets, m_RenderTarget.width, m_RenderTarget.height, scene, this, renderChannel);

            for (int j = 0; j < m_RenderTarget.height; j += 32)
            {
                for (int i = 0; i < m_RenderTarget.width; i += 32)
                {
                    job.AddTile(i, j);
                }
            }
            job.Render(m_RenderTarget, progressCallBackAction);

        }

        internal Color RenderPixelToColor(int x, int y, SamplerBase sampler, RenderState renderState, RenderChannel renderChannel, Scene scene)
        {
            if (renderChannel == RenderChannel.Full)
            {
                Color col = Color.black;
                sampler.ResetSampler(); //重置采样器状态
                renderState.ResetState(); //重置渲染状态
                while (sampler.NextSample())
                {
                    Ray ray = GetRay(x, y, sampler);
                    col += scene.tracer.Tracing(ray, sampler, renderState);
                }

                col.a = 1.0f;
                return col;
            }
            else
            {
                //如果只渲染某个通道，则调用PreviewTracing
                Ray ray = GetRayWithoutSampler(x + 0.5f, y + 0.5f);
                return scene.tracer.PreviewTracing(ray, renderChannel);
            }
        }
    }
}
