using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
	/// <summary>
	/// 几何体基类
	/// </summary>
    public abstract class Geometry : IAABB
    {
        public Material material;

        protected Bounds m_bounds;

        public Geometry(Material material)
        {
            this.material = material;
        }

        public Bounds GetBounds()
        {
            return m_bounds;
        }

        public bool RayCast(Ray ray, ref RayCastHit hit)
        {
            if(RayCastGeometry(ray, ref hit))
            {
                //如果射线检测成功，则额外判断一下shader是否允许cull，用于实现材质的alpha裁剪等功能
                if (hit.material != null && hit.material.ShouldCull(ray, hit))
                    return false;
                return true;
            }
            return false;
        }

        public abstract Vector3 Sample(SamplerBase sampler);

        public abstract Vector3 GetNormal(Vector3 point);

        public abstract float GetPDF();

        protected abstract bool RayCastGeometry(Ray ray, ref RayCastHit hit);
    }
}
