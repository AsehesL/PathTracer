using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    class ListSceneData : SceneData
    {
        private List<BoundsGeometry> m_BoundsGeometries;
        private Bounds m_Bounds;

        public ListSceneData() : base()
        {
        }

        protected override void BuildForBoundsGeometries(List<BoundsGeometry> boundsGeometries, Bounds bounds)
        {
            m_BoundsGeometries = boundsGeometries;
            m_Bounds = bounds;
        }

        protected override bool RaycastTriangles(Ray ray, double epsilon, ref RayCastHit hit)
        {
            if (m_BoundsGeometries == null)
                return false;
            if (!m_Bounds.Raycast(ray))
                return false;
			bool result = false;
			for (int i = 0; i < m_BoundsGeometries.Count; i++)
			{
				if (m_BoundsGeometries[i].RayCast(ray, epsilon, ref hit))
				{
					result = true;
				}
			}

			return result;
		}
    }
}
