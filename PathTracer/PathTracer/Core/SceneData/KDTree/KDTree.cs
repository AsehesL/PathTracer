using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    class KDTree : SceneData
    {
        private Bounds m_Bounds;

        private KDTreeNode m_Root;

        public override void Build(List<Geometry> geometries)
        {
            if (geometries.Count <= 0)
                return;

            m_Bounds = geometries[0].bounds;

            for (int i = 1; i < geometries.Count; i++)
            {
                m_Bounds.Encapsulate(geometries[i].bounds);
            }
            m_Bounds.size *= 1.1;

            m_Root = KDTreeNode.Build(geometries, m_Bounds);
        }

        public override bool Raycast(Ray ray, double epsilon, out RayCastHit hit)
        {
            hit = default(RayCastHit);
            hit.distance = double.MaxValue;
            if (m_Root == null)
                return false;

            if (m_Bounds.Raycast(ray) == false)
            {
                return false;
            }

            //if (m_Root != null)
            {
                return m_Root.Raycast(ray, epsilon, ref hit);
            }

            //return false;
        }
    }
}
