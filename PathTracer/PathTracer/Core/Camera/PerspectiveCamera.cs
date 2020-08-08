

using System;

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ASL.PathTracer
{
    /// <summary>
    /// 透视相机
    /// </summary>
    public class PerspectiveCamera : CameraBase
    {

        public double near { get; private set; }

        public double fieldOfView { get; private set; }

        /// <summary>
        /// 透镜半径
        /// </summary>
        public float radius;

        /// <summary>
        /// 焦距
        /// </summary>
        public float focal;

        /// <summary>
        /// 启用薄透镜
        /// </summary>
        public bool useThinLens;

        private double m_Height;
        private double m_Width;

        public PerspectiveCamera(Vector3 position, Vector3 euler, double near, double fieldOfView) : base(position, euler)
        {
            this.near = near;
            this.fieldOfView = fieldOfView;
        }

        protected override void ModifyResolution(uint width, uint height)
        {
            base.ModifyResolution(width, height);

            float aspect = (((float)width) / height);

            m_Height = near * Math.Tan(fieldOfView * 0.5 * 0.01745329252);
            m_Width = aspect * m_Height;
        }

        public override Ray GetRay(int x, int y, SamplerBase sampler)
        {
            if(useThinLens)
            {
                return GetThinLensRayFromPixel(x, y, sampler);
            }
            var sample = sampler.Sample();
            return GetRayFromPixel(sample.x + x, sample.y + y);
        }

        //public override Ray GetRayWithoutSampler(float x, float y)
        //{
        //    return GetRayFromPixel(x, y);
        //}

        /// <summary>
        /// 根据焦距和透镜半径计算光线
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="sampler"></param>
        /// <returns></returns>
        private Ray GetThinLensRayFromPixel(int x, int y, SamplerBase sampler)
        {
            if (m_RenderTarget == null)
                throw new System.NullReferenceException();
            var sample = sampler.Sample();
            double px = (m_RenderTarget.width - 1 - (sample.x + x)) / m_RenderTarget.width * 2 - 1;
            double py = (sample.y + y) / m_RenderTarget.height * 2 - 1;
            px *= m_Width;
            py *= m_Height;
            double per = focal / near;
            px = px * per;
            py = py * per;
            Vector3 p = position + right * px + up * py + forward * focal;

            Vector2 disk = sampler.SampleUnitDisk();

            Vector3 ori = position + right * disk.x * radius + up * disk.y * radius;

            Vector3 dir = (p - ori).normalized;
            return new Ray(ori, dir);
        }

        private Ray GetRayFromPixel(double x, double y)
        {
            if (m_RenderTarget == null)
                throw new System.NullReferenceException();
            x = (m_RenderTarget.width - 1 - x) / m_RenderTarget.width * 2 - 1;
            y = y / m_RenderTarget.height * 2 - 1;

            Vector2 point = new Vector2(x * m_Width, y * m_Height);
            return GetRayFromPoint(point);
        }

        private Ray GetRayFromPoint(Vector2 point)
        {
            Vector3 dir = right * point.x + up * point.y + forward * this.near;
            Vector3 ori = this.position + dir;
            dir.Normalize();
            return new Ray(ori, dir);
        }
    }
}