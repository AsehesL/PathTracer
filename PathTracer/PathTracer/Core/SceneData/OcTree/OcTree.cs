//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ASL.PathTracer
//{
//    class OcTree : SceneData
//    {
//        private int m_MaxDepth;

//        private OcTreeNode m_Node;

//        public OcTree(int depth = 7)
//        {
//            m_MaxDepth = depth;
//        }

//        protected override void BuildForTriangles(List<Triangle> triangles, Bounds bounds)
//        {

//            bounds.size *= 1.1;

//            m_Node = new OcTreeNode(bounds);

//            for (int i = 0; i < triangles.Count; i++)
//            {
//                m_Node.Insert(triangles[i], 0, m_MaxDepth);
//            }
//        }

//        protected override bool RaycastTriangles(Ray ray, double epsilon, ref RayCastHit hit)
//        {
//            hit = default(RayCastHit);
//            hit.distance = double.MaxValue;
//            return RaycastInOcTree(ray, epsilon, m_Node, ref hit);
//        }

//        private static bool RaycastInOcTree(Ray ray, double epsilon, OcTreeNode node, ref RayCastHit hit)
//        {
//            bool raycast = false;
//            if (node.bounds.Raycast(ray))
//            {
//                for (int i = 0; i < node.triangles.Count; i++)
//                {
//                    Triangle triangle = node.triangles[i];

//                    if (triangle.RayCast(ray, epsilon, ref hit))
//                    {
//                        raycast = true;
//                    }
//                }
//                for (int i = 0; i < 8; i++)
//                {
//                    var child = node.GetChild(i);
//                    if (child != null)
//                    {
//                        if (RaycastInOcTree(ray, epsilon, child, ref hit))
//                        {
//                            raycast = true;
//                        }
//                    }
//                }
//            }

//            return raycast;

//        }

//		class OcTreeNode
//		{
//			public Bounds bounds;

//			public List<Triangle> triangles
//			{
//				get { return m_Triangles; }
//			}

//			public OcTreeNode GetChild(int index)
//			{
//				if (index == 0)
//					return m_ForwardLeftBottom;
//				else if (index == 1)
//					return m_ForwardLeftTop;
//				else if (index == 2)
//					return m_ForwardRightBottom;
//				else if (index == 3)
//					return m_ForwardRightTop;
//				else if (index == 4)
//					return m_BackLeftBottom;
//				else if (index == 5)
//					return m_BackLeftTop;
//				else if (index == 6)
//					return m_BackRightBottom;
//				else if (index == 7)
//					return m_BackRightTop;
//				return null;
//			}

//			private OcTreeNode m_ForwardLeftTop;
//			private OcTreeNode m_ForwardLeftBottom;
//			private OcTreeNode m_ForwardRightTop;
//			private OcTreeNode m_ForwardRightBottom;
//			private OcTreeNode m_BackLeftTop;
//			private OcTreeNode m_BackLeftBottom;
//			private OcTreeNode m_BackRightTop;
//			private OcTreeNode m_BackRightBottom;

//			private List<Triangle> m_Triangles;

//			public OcTreeNode(Bounds bounds)
//			{
//				this.bounds = bounds;
//				this.m_Triangles = new List<Triangle>();
//			}

//			public OcTreeNode Insert(Triangle triangle, int depth, int maxDepth)
//			{
//				if (depth < maxDepth)
//				{
//					OcTreeNode node = GetContainerNode(triangle);
//					if (node != null)
//						return node.Insert(triangle, depth + 1, maxDepth);
//				}
//				m_Triangles.Add(triangle);
//				return this;
//			}

//			private OcTreeNode GetContainerNode(Triangle triangle)
//			{
//				Vector3 halfSize = bounds.size * 0.5;
//				OcTreeNode result = null;
//				result = GetContainerNode(ref m_ForwardLeftBottom,
//					bounds.center + new Vector3(-halfSize.x * 0.5, halfSize.y * 0.5, halfSize.z * 0.5),
//					halfSize, triangle);
//				if (result != null)
//					return result;

//				result = GetContainerNode(ref m_ForwardLeftTop,
//					bounds.center + new Vector3(-halfSize.x * 0.5, halfSize.y * 0.5, -halfSize.z * 0.5),
//					halfSize, triangle);
//				if (result != null)
//					return result;

//				result = GetContainerNode(ref m_ForwardRightBottom,
//					bounds.center + new Vector3(halfSize.x * 0.5, halfSize.y * 0.5, halfSize.z * 0.5),
//					halfSize, triangle);
//				if (result != null)
//					return result;

//				result = GetContainerNode(ref m_ForwardRightTop,
//					bounds.center + new Vector3(halfSize.x * 0.5, halfSize.y * 0.5, -halfSize.z * 0.5),
//					halfSize, triangle);
//				if (result != null)
//					return result;

//				result = GetContainerNode(ref m_BackLeftBottom,
//					bounds.center + new Vector3(-halfSize.x * 0.5, -halfSize.y * 0.5, halfSize.z * 0.5),
//					halfSize, triangle);
//				if (result != null)
//					return result;

//				result = GetContainerNode(ref m_BackLeftTop,
//					bounds.center + new Vector3(-halfSize.x * 0.5, -halfSize.y * 0.5, -halfSize.z * 0.5),
//					halfSize, triangle);
//				if (result != null)
//					return result;

//				result = GetContainerNode(ref m_BackRightBottom,
//					bounds.center + new Vector3(halfSize.x * 0.5, -halfSize.y * 0.5, halfSize.z * 0.5),
//					halfSize, triangle);
//				if (result != null)
//					return result;

//				result = GetContainerNode(ref m_BackRightTop,
//					bounds.center + new Vector3(halfSize.x * 0.5, -halfSize.y * 0.5, -halfSize.z * 0.5),
//					halfSize, triangle);
//				if (result != null)
//					return result;

//				return null;
//			}

//			private OcTreeNode GetContainerNode(ref OcTreeNode node, Vector3 centerPos, Vector3 size, Triangle triangle)
//			{
//				OcTreeNode result = null;
//				Bounds bd = triangle.bounds;
//				if (node == null)
//				{
//					Bounds bounds = new Bounds(centerPos, size);
//					if (bounds.Contains(bd))
//					{
//						node = new OcTreeNode(bounds);
//						result = node;
//					}
//				}
//				else if (node.bounds.Contains(bd))
//				{
//					result = node;
//				}
//				return result;
//			}
//		}
//	}
//}
