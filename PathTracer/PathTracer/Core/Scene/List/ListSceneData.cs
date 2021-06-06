using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    /// <summary>
    /// 简单列表场景数据（仅用于测试）
    /// </summary>
    class ListSceneData : SceneData
    {
        private List<Geometry> m_Geometries;

        private Bounds m_Bounds;

        public ListSceneData() : base()
        {
        }

        public override Bounds GetBounds()
        {
            return m_Bounds;
        }

        public override void Build(List<Geometry> geometries)
        {
            if (m_Geometries == null)
                m_Geometries = new List<Geometry>(geometries.Count);
            else
                m_Geometries.Clear();
            Vector3 min = Vector3.one * double.MaxValue;
            Vector3 max = Vector3.one * double.MinValue;
            for (int i = 0; i < geometries.Count; i++)
            {
                var geometry = geometries[i];
                if (geometry != null)
                {
                    min = Vector3.Min(min, geometry.GetBounds().min);
                    max = Vector3.Max(max, geometry.GetBounds().max);

                    m_Geometries.Add(geometry);
                }
            }

            if (m_Geometries.Count > 0)
            {
                m_Bounds = new Bounds((min + max) * 0.5, max - min);
            }
        }

        protected override bool RaycastTriangles(Ray ray, ref RayCastHit hit)
        {
            if (m_Geometries == null)
                return false;
            if (!m_Bounds.Raycast(ray))
                return false;
			bool result = false;
			for (int i = 0; i < m_Geometries.Count; i++)
			{
				if (m_Geometries[i].RayCast(ray, ref hit))
				{
					result = true;
				}
			}

			return result;
		}
    }
}
