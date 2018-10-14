using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    class OcTree : SceneData
    {
        private OcTreeNode m_Node;

        public override void Build(List<Geometry> geometries)
        {
            Bounds bounds = geometries[0].bounds;
            for (int i = 1; i < geometries.Count; i++)
            {
                bounds.Encapsulate(geometries[i].bounds);
            }

            m_Node = new OcTreeNode(bounds);

            for (int i = 0; i < geometries.Count; i++)
            {
                m_Node.Insert(geometries[i], 0, 7);
            }
        }

        public override bool Raycast(Ray ray, double epsilon, out RayCastHit hit)
        {
            hit = default(RayCastHit);
            hit.distance = double.MaxValue;
            return RaycastInOcTree(ray, epsilon, m_Node, ref hit);
        }

        private static bool RaycastInOcTree(Ray ray, double epsilon, OcTreeNode node, ref RayCastHit hit)
        {
            bool raycast = false;
            if (node.bounds.Raycast(ray))
            {
                for (int i = 0; i < node.geometries.Count; i++)
                {
                    Geometry geometry = node.geometries[i];

                    if (geometry.RayCast(ray, epsilon, ref hit))
                    {
                        raycast = true;
                    }
                }
                for (int i = 0; i < 8; i++)
                {
                    var child = node.GetChild(i);
                    if (child != null)
                    {
                        if (RaycastInOcTree(ray, epsilon, child, ref hit))
                        {
                            raycast = true;
                        }
                    }
                }
            }

            return raycast;

        }
    }
}
