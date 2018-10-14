using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    class ListSceneData : SceneData
    {
        private List<Geometry> m_Geometrys;

        public ListSceneData() : base()
        {
            m_Geometrys = new List<Geometry>();
        }

        public override void Build(List<Geometry> geometries)
        {
            m_Geometrys = geometries;
        }

        public override bool Raycast(Ray ray, double epsilon, out RayCastHit hit)
        {
            hit = default(RayCastHit);
            hit.distance = double.MaxValue;
            bool result = false;
            for (int i = 0; i < m_Geometrys.Count; i++)
            {
                if (m_Geometrys[i].RayCast(ray, epsilon, ref hit))
                {
                    result = true;
                }
            }

            return result;
        }
    }
}
