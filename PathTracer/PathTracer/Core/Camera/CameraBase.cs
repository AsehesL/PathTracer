using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
  
    /// <summary>
    /// 相机基类
    /// </summary>
    public abstract class CameraBase
    {
        public Vector3 position { get; protected set; }

        public Vector3 right { get; private set; }

        public Vector3 up { get; private set; }

        public Vector3 forward { get; private set; }

        public Texture renderTarget { get; private set; }

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
            this.renderTarget = renderTarget;
            if (this.renderTarget == null)
                return;

            ModifyResolution(renderTarget.width, renderTarget.height);
        }

        protected virtual void ModifyResolution(uint width, uint height) { }

        public abstract Ray GetRay(int x, int y, SamplerBase sampler);
    }
}
