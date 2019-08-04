using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    class ListSceneData : SceneData
    {
        private List<Triangle> m_Triangles;

        public ListSceneData() : base()
        {
	        m_Triangles = new List<Triangle>();
        }

		protected override void BuildForTriangles(List<Triangle> triangles)
		{
			m_Triangles = triangles;
		}

		protected override bool RaycastTriangles(Ray ray, double epsilon, ref RayCastHit hit)
		{
			bool result = false;
			for (int i = 0; i < m_Triangles.Count; i++)
			{
				if (m_Triangles[i].RayCast(ray, epsilon, ref hit))
				{
					result = true;
				}
			}

			return result;
		}
    }
}
