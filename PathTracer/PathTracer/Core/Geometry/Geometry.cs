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
    public abstract class Geometry
    {
        public Shader shader;

        public Geometry(Shader shader)
        {
            this.shader = shader;
        }

        public bool RayCast(Ray ray, double epsilon, ref RayCastHit hit)
        {
            if(RayCastGeometry(ray, epsilon, ref hit))
            {
                //如果射线检测成功，则额外判断一下shader是否允许cull，用于实现材质的alpha裁剪等功能
                if (hit.shader != null && hit.shader.ShouldCull(ray, hit, epsilon))
                    return false;
                return true;
            }
            return false;
        }

        protected abstract bool RayCastGeometry(Ray ray, double epsilon, ref RayCastHit hit);
    }
}
