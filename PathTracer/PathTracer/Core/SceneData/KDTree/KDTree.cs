//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ASL.PathTracer
//{
	
//	/// <summary>
//	/// KD-Tree
//	/// </summary>
//	class KDTree : SceneData
//	{
//		private int m_MaxDepth;

//		private KDTreeNode m_Root;

//		private enum Axis
//		{
//			X,
//			Y,
//			Z
//		}

//		private struct Plane
//		{
//			public Axis axis;
//			public double value;
//		}

//		public KDTree(int depth = 7)
//		{
//			m_MaxDepth = depth;
//		}

//	    protected override void BuildForBoundsGeometries(List<BoundsGeometry> boundsGeometries, Bounds bounds)
//	    {
//	        m_Root = BuildTree(boundsGeometries, 0);
//        }

//	    protected override bool RaycastTriangles(Ray ray, double epsilon, ref RayCastHit hit)
//		{
//			bool raycast = false;
//			if (m_Root.IsLeaf)
//			{
//				for (int i = 0; i < m_Root.Triangles.Count; i++)
//				{
//					raycast = m_Root.Triangles[i].RayCast(ray, epsilon, ref hit) || raycast;
//				}

//				return raycast;
//			}

//            Stack<KDTreeNode> nodes = new Stack<KDTreeNode>();
//			if (m_Root.Bounds.Raycast(ray))
//			{
//				nodes.Push(m_Root);
//			}

//			while (nodes.Count > 0)
//            {
//                var node = nodes.Pop();
//	            if (node.IsLeaf == false)
//	            {
//		            double hitleftdistance = 0.0, hitrightdistance = 0.0;
//		            bool hitleft = node.LeftNode != null ? node.LeftNode.Bounds.Raycast(ray, out hitleftdistance) : false;
//		            bool hitright = node.RightNode != null ? node.RightNode.Bounds.Raycast(ray, out hitrightdistance) : false;
//		            if (hitleft && hitright)
//		            {
//			            if (hitleftdistance < hitrightdistance)
//			            {
//				            nodes.Push(node.RightNode);
//				            nodes.Push(node.LeftNode);
//						}
//			            else
//			            {
//				            nodes.Push(node.LeftNode);
//				            nodes.Push(node.RightNode);
//						}
//		            }
//		            else if (hitleft)
//			            nodes.Push(node.LeftNode);
//					else if (hitright)
//			            nodes.Push(node.RightNode);
//	            }
//	            else
//                {
//                    if (node.Triangles != null)
//                    {
//                        for (int i = 0; i < node.Triangles.Count; i++)
//                        {
//                            raycast = node.Triangles[i].RayCast(ray, epsilon, ref hit) || raycast;
//                        }
//                    }
//                }
//            }

//			return raycast;
//		}

//		private KDTreeNode BuildTree(List<BoundsGeometry> boundsGeometries, int depth)
//		{
//			if (boundsGeometries == null || boundsGeometries.Count == 0)
//				return null;
//			if (depth >= m_MaxDepth)
//				return new KDTreeNode(boundsGeometries);//达到指定深度直接结束递归并返回叶节点

//			Plane plane = PickKDTreePlane(boundsGeometries);//计算分割面

//			List<Triangle> left = new List<Triangle>();
//			List<Triangle> right = new List<Triangle>();

//			for (int i = 0; i < boundsGeometries.Count; i++)
//			{
//				SplitTriangles(boundsGeometries[i], plane, left, right);//对三角面进行分组，如果位于分割面上，需要对三角形进行切割
//			}

//			//KDTreeNode node = new KDTreeNode();
			
//			//建立左右子树
//			KDTreeNode leftnode = BuildTree(left, depth + 1);
//			KDTreeNode rightnode = BuildTree(right, depth + 1);

//			if (leftnode == null && rightnode == null)
//				return null;
//			//node.SetNode(leftnode, rightnode);

//			return new KDTreeNode(leftnode, rightnode);
//		}

//		private Plane PickKDTreePlane(List<Triangle> triangles)
//		{
//			Vector3 min = Vector3.one * double.MaxValue;
//			Vector3 max = -1.0 * Vector3.one * double.MaxValue;

//			for (int i = 0; i < triangles.Count; i++)
//			{
//				min = Vector3.Min(min, triangles[i].vertex0.position);
//				max = Vector3.Max(max, triangles[i].vertex0.position);
//				min = Vector3.Min(min, triangles[i].vertex1.position);
//				max = Vector3.Max(max, triangles[i].vertex1.position);
//				min = Vector3.Min(min, triangles[i].vertex2.position);
//				max = Vector3.Max(max, triangles[i].vertex2.position);
//			}

//			Vector3 size = max - min;

//			Axis axis = default(Axis);
//			double value = default(double);
//			if (size.x >= size.y && size.x >= size.z)
//			{
//				axis = Axis.X;
//				value = min.x + size.x * 0.5;
//			}
//			else if (size.y > size.x && size.y >= size.z)
//			{
//				axis = Axis.Y;
//				value = min.y + size.y * 0.5;
//			}
//			else if (size.z > size.x && size.z > size.y)
//			{
//				axis = Axis.Z;
//				value = min.z + size.z * 0.5;
//			}

//			return new Plane { axis = axis, value = value };
//		}

//		private void SplitTriangles(Triangle triangle, Plane plane, List<Triangle> left, List<Triangle> right)
//		{
//			double pos0 = 0, pos1 = 0, pos2 = 0;
//			if (plane.axis == Axis.X)
//			{
//				pos0 = triangle.vertex0.position.x;
//				pos1 = triangle.vertex1.position.x;
//				pos2 = triangle.vertex2.position.x;
//			}
//			else if (plane.axis == Axis.Y)
//			{
//				pos0 = triangle.vertex0.position.y;
//				pos1 = triangle.vertex1.position.y;
//				pos2 = triangle.vertex2.position.y;
//			}
//			else if (plane.axis == Axis.Z)
//			{
//				pos0 = triangle.vertex0.position.z;
//				pos1 = triangle.vertex1.position.z;
//				pos2 = triangle.vertex2.position.z;
//			}

//			if (pos0 <= plane.value && pos1 <= plane.value && pos2 <= plane.value)
//			{
//				left.Add(triangle);
//			}
//			else if (pos0 > plane.value && pos1 > plane.value && pos2 > plane.value)
//			{
//				right.Add(triangle);
//			}
//			else
//			{
//				List<Vertex> leftVertices = new List<Vertex>();
//				List<Vertex> rightVertices = new List<Vertex>();
//				SplitEdge(triangle.vertex0, triangle.vertex1, plane, leftVertices, rightVertices);
//				SplitEdge(triangle.vertex1, triangle.vertex2, plane, leftVertices, rightVertices);
//				SplitEdge(triangle.vertex2, triangle.vertex0, plane, leftVertices, rightVertices);
//				if (leftVertices.Count == 4)
//				{
//					left.Add(new Triangle(leftVertices[0], leftVertices[1], leftVertices[2], triangle.shader));
//					left.Add(new Triangle(leftVertices[0], leftVertices[2], leftVertices[3], triangle.shader));
//				}
//				else if (leftVertices.Count == 3)
//				{
//					left.Add(new Triangle(leftVertices[0], leftVertices[1], leftVertices[2], triangle.shader));
//				}

//				if (rightVertices.Count == 4)
//				{
//					right.Add(new Triangle(rightVertices[0], rightVertices[1], rightVertices[2], triangle.shader));
//					right.Add(new Triangle(rightVertices[0], rightVertices[2], rightVertices[3], triangle.shader));
//				}
//				else if (rightVertices.Count == 3)
//				{
//					right.Add(new Triangle(rightVertices[0], rightVertices[1], rightVertices[2], triangle.shader));
//				}
//			}
//		}

//		private void SplitEdge(Vertex begin, Vertex end, Plane plane, List<Vertex> leftVertices, List<Vertex> rightVertices)
//		{
//			double beginPos = 0, endPos = 0;
//			if (plane.axis == Axis.X)
//			{
//				beginPos = begin.position.x;
//				endPos = end.position.x;
//			}
//			else if (plane.axis == Axis.Y)
//			{
//				beginPos = begin.position.y;
//				endPos = end.position.y;
//			}
//			else if (plane.axis == Axis.Z)
//			{
//				beginPos = begin.position.z;
//				endPos = end.position.z;
//			}

//			if (beginPos <= plane.value && endPos <= plane.value)
//			{
//				leftVertices.Add(begin);
//			}
//			else if (beginPos > plane.value && endPos > plane.value)
//			{
//				rightVertices.Add(begin);
//			}
//			else if (beginPos <= plane.value && endPos > plane.value)
//			{
//				leftVertices.Add(begin);
//				leftVertices.Add(Vertex.Lerp(begin, end, (plane.value - beginPos) / (endPos - beginPos)));
//				rightVertices.Add(Vertex.Lerp(begin, end, (plane.value - beginPos) / (endPos - beginPos)));
//			}
//			else if (beginPos > plane.value && endPos <= plane.value)
//			{
//				rightVertices.Add(begin);
//				rightVertices.Add(Vertex.Lerp(begin, end, (plane.value - beginPos) / (endPos - beginPos)));
//				leftVertices.Add(Vertex.Lerp(begin, end, (plane.value - beginPos) / (endPos - beginPos)));
//			}
//		}

//		class KDTreeNode
//		{

//			public KDTreeNode LeftNode { get; private set; }

//			public KDTreeNode RightNode { get; private set; }

//			public Bounds Bounds { get; private set; }

//			public bool IsLeaf { get; private set; }

//			public List<Triangle> Triangles { get; private set; }

//			public KDTreeNode(KDTreeNode left, KDTreeNode right)
//			{
//				IsLeaf = false;
//				LeftNode = left;
//				RightNode = right;

//				Vector3 min = default(Vector3), max = default(Vector3);
//				if (left != null)
//				{
//					min = left.Bounds.min;
//					max = left.Bounds.max;
//					if (right != null)
//					{
//						min = Vector3.Min(min, right.Bounds.min);
//						max = Vector3.Max(max, right.Bounds.max);
//					}
//				}
//				else if (right != null)
//				{
//					min = right.Bounds.min;
//					max = right.Bounds.max;
//				}

//				Vector3 si = max - min;
//				Vector3 ct = min + si * 0.5;

//				if (si.x <= 0)
//					si.x = 0.1;
//				if (si.y <= 0)
//					si.y = 0.1;
//				if (si.z <= 0)
//					si.z = 0.1;

//				this.Bounds = new Bounds(ct, si);
//			}

//			public KDTreeNode(List<Triangle> triangles)
//			{
//				IsLeaf = true;
//				Triangles = triangles;

//				Vector3 min = Vector3.one * double.MaxValue;
//				Vector3 max = Vector3.one * -double.MaxValue;

//				for (int i = 0; i < triangles.Count; i++)
//				{
//					min = Vector3.Min(min, triangles[i].bounds.min);
//					max = Vector3.Max(max, triangles[i].bounds.max);
//				}

//				Vector3 si = max - min;
//				Vector3 ct = min + si * 0.5;

//				if (si.x <= 0)
//					si.x = 0.1;
//				if (si.y <= 0)
//					si.y = 0.1;
//				if (si.z <= 0)
//					si.z = 0.1;

//				this.Bounds = new Bounds(ct, si);
//			}
//		}
//	}
//}
