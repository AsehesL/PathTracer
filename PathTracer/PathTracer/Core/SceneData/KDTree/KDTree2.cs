using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
	enum KDTreeAxis
	{
		X,
		Y,
		Z
	}

	struct KDTreePlane
	{
		public KDTreeAxis axis;
		public double value;
	}

	class KDTree2 : SceneData
	{
		private int m_MaxDepth;

		private KDTreeNode m_Root;

		public KDTree2(int depth)
		{
			m_MaxDepth = depth;
		}

		protected override void BuildForTriangles(List<Triangle> triangles)
		{
			m_Root = BuildTree(triangles, 0);
		}

		protected override bool RaycastTriangles(Ray ray, double epsilon, ref RayCastHit hit)
		{
			return m_Root.Raycast(ray, epsilon, ref hit);
		}

		private KDTreeNode BuildTree(List<Triangle> triangles, int depth)
		{
			if (triangles == null || triangles.Count == 0)
				return null;
			if (depth >= m_MaxDepth)
				return new KDTreeLeaf(triangles);
			KDTreePlane plane = PickKDTreePlane(triangles);

			List<Triangle> left = new List<Triangle>();
			List<Triangle> right = new List<Triangle>();

			for (int i = 0; i < triangles.Count; i++)
			{
				SplitTriangles(triangles[i], plane, left, right);
			}

			KDTreeNode node = new KDTreeNode();

			node.plane = plane;
			KDTreeNode leftnode = BuildTree(left, depth + 1);
			KDTreeNode rightnode = BuildTree(right, depth + 1);

			node.left = leftnode;
			node.right = rightnode;

			return node;
		}

		private KDTreePlane PickKDTreePlane(List<Triangle> triangles)
		{
			Vector3 min = Vector3.one * double.MaxValue;
			Vector3 max = -1.0 * Vector3.one * double.MaxValue;

			for (int i = 0; i < triangles.Count; i++)
			{
				min = Vector3.Min(min, triangles[i].vertex0.position);
				max = Vector3.Max(max, triangles[i].vertex0.position);
				min = Vector3.Min(min, triangles[i].vertex1.position);
				max = Vector3.Max(max, triangles[i].vertex1.position);
				min = Vector3.Min(min, triangles[i].vertex2.position);
				max = Vector3.Max(max, triangles[i].vertex2.position);
			}

			Vector3 size = max - min;

			KDTreeAxis axis = default(KDTreeAxis);
			double value = default(double);
			if (size.x >= size.y && size.x >= size.z)
			{
				axis = KDTreeAxis.X;
				value = min.x + size.x * 0.5;
			}
			else if (size.y > size.x && size.y >= size.z)
			{
				axis = KDTreeAxis.Y;
				value = min.y + size.y * 0.5;
			}
			else if (size.z > size.x && size.z > size.y)
			{
				axis = KDTreeAxis.Z;
				value = min.z + size.z * 0.5;
			}

			return new KDTreePlane { axis = axis, value = value };
		}

		private void SplitTriangles(Triangle triangle, KDTreePlane plane, List<Triangle> left, List<Triangle> right)
		{
			double pos0 = 0, pos1 = 0, pos2 = 0;
			if (plane.axis == KDTreeAxis.X)
			{
				pos0 = triangle.vertex0.position.x;
				pos1 = triangle.vertex1.position.x;
				pos2 = triangle.vertex2.position.x;
			}
			else if (plane.axis == KDTreeAxis.Y)
			{
				pos0 = triangle.vertex0.position.y;
				pos1 = triangle.vertex1.position.y;
				pos2 = triangle.vertex2.position.y;
			}
			else if (plane.axis == KDTreeAxis.Z)
			{
				pos0 = triangle.vertex0.position.z;
				pos1 = triangle.vertex1.position.z;
				pos2 = triangle.vertex2.position.z;
			}

			if (pos0 <= plane.value && pos1 <= plane.value && pos2 <= plane.value)
			{
				left.Add(triangle);
			}
			else if (pos0 > plane.value && pos1 > plane.value && pos2 > plane.value)
			{
				right.Add(triangle);
			}
			else
			{
				List<Vertex> leftVertices = new List<Vertex>();
				List<Vertex> rightVertices = new List<Vertex>();
				SplitEdge(triangle.vertex0, triangle.vertex1, plane, leftVertices, rightVertices);
				SplitEdge(triangle.vertex1, triangle.vertex2, plane, leftVertices, rightVertices);
				SplitEdge(triangle.vertex2, triangle.vertex0, plane, leftVertices, rightVertices);
				if (leftVertices.Count == 4)
				{
					left.Add(new Triangle(leftVertices[0], leftVertices[1], leftVertices[2], triangle.shader));
					left.Add(new Triangle(leftVertices[0], leftVertices[2], leftVertices[3], triangle.shader));
				}
				else if (leftVertices.Count == 3)
				{
					left.Add(new Triangle(leftVertices[0], leftVertices[1], leftVertices[2], triangle.shader));
				}

				if (rightVertices.Count == 4)
				{
					right.Add(new Triangle(rightVertices[0], rightVertices[1], rightVertices[2], triangle.shader));
					right.Add(new Triangle(rightVertices[0], rightVertices[2], rightVertices[3], triangle.shader));
				}
				else if (rightVertices.Count == 3)
				{
					right.Add(new Triangle(rightVertices[0], rightVertices[1], rightVertices[2], triangle.shader));
				}
			}
		}

		private void SplitEdge(Vertex begin, Vertex end, KDTreePlane plane, List<Vertex> leftVertices, List<Vertex> rightVertices)
		{
			double beginPos = 0, endPos = 0;
			if (plane.axis == KDTreeAxis.X)
			{
				beginPos = begin.position.x;
				endPos = end.position.x;
			}
			else if (plane.axis == KDTreeAxis.Y)
			{
				beginPos = begin.position.y;
				endPos = end.position.y;
			}
			else if (plane.axis == KDTreeAxis.Z)
			{
				beginPos = begin.position.z;
				endPos = end.position.z;
			}

			if (beginPos <= plane.value && endPos <= plane.value)
			{
				leftVertices.Add(begin);
			}
			else if (beginPos > plane.value && endPos > plane.value)
			{
				rightVertices.Add(begin);
			}
			else if (beginPos <= plane.value && endPos > plane.value)
			{
				leftVertices.Add(begin);
				leftVertices.Add(Vertex.Lerp(begin, end, (plane.value - beginPos) / (endPos - beginPos)));
				rightVertices.Add(Vertex.Lerp(begin, end, (plane.value - beginPos) / (endPos - beginPos)));
			}
			else if (beginPos > plane.value && endPos <= plane.value)
			{
				rightVertices.Add(begin);
				rightVertices.Add(Vertex.Lerp(begin, end, (plane.value - beginPos) / (endPos - beginPos)));
				leftVertices.Add(Vertex.Lerp(begin, end, (plane.value - beginPos) / (endPos - beginPos)));
			}
		}
	}

	class KDTreeNode
	{
		public KDTreeNode left;
		public KDTreeNode right;
		public KDTreePlane plane;

		protected bool m_IsDrawing = true;

		public virtual bool Raycast(Ray ray, double epsilon, ref RayCastHit hit)
		{
            double or = 0;
            double d = 0;
            if (plane.axis == KDTreeAxis.X)
            {
                or = ray.origin.x;
                d = ray.direction.x;
            }
            else if (plane.axis == KDTreeAxis.Y)
            {
                or = ray.origin.y;
                d = ray.direction.y;
            }
            else if (plane.axis == KDTreeAxis.Z)
            {
                or = ray.origin.z;
                d = ray.direction.z;
            }

            if (or <= plane.value && d <= 0)
                return left.Raycast(ray, epsilon, ref hit);
            if (or > plane.value && d >= 0)
                return right.Raycast(ray, epsilon, ref hit);

            bool ishit = false;

		    //GetPlaneHitPos(ray, out var leftray, out var rightray);

            ishit = left.Raycast(ray, epsilon, ref hit);
            ishit = right.Raycast(ray, epsilon, ref hit) || ishit;

			return ishit;
		}

		private void GetPlaneHitPos(Ray ray, out Ray leftray, out Ray rightray)
		{
			if (plane.axis == KDTreeAxis.X)
			{
				double t = (plane.value - ray.origin.x) / ray.direction.x;
				Vector3 hitpoint = ray.origin + ray.direction * t - ray.direction.normalized * 0.01;
				if (ray.origin.x <= plane.value)
				{
					leftray = ray;
					rightray = new Ray(hitpoint, ray.direction);
				}
				else
				{
					leftray = new Ray(hitpoint, ray.direction);
					rightray = ray;
				}

				return;
			}
			if (plane.axis == KDTreeAxis.Y)
			{
				double t = (plane.value - ray.origin.y) / ray.direction.y;
				Vector3 hitpoint = ray.origin + ray.direction * t - ray.direction.normalized * 0.01;
				if (ray.origin.y <= plane.value)
				{
					leftray = ray;
					rightray = new Ray(hitpoint, ray.direction);
				}
				else
				{
					leftray = new Ray(hitpoint, ray.direction);
					rightray = ray;
				}

				return;
			}
			if (plane.axis == KDTreeAxis.Z)
			{
				double t = (plane.value - ray.origin.z) / ray.direction.z;
				Vector3 hitpoint = ray.origin + ray.direction * t - ray.direction.normalized * 0.01;
				if (ray.origin.z <= plane.value)
				{
					leftray = ray;
					rightray = new Ray(hitpoint, ray.direction);
				}
				else
				{
					leftray = new Ray(hitpoint, ray.direction);
					rightray = ray;
				}

				return;
			}

			leftray = default(Ray);
			rightray = default(Ray);
		}
	}

	class KDTreeLeaf : KDTreeNode
	{
		public List<Triangle> triangles;

		public KDTreeLeaf(List<Triangle> triangles)
		{
			this.triangles = triangles;
		}

		public override bool Raycast(Ray ray, double epsilon, ref RayCastHit hit)
		{
			bool ishit = false;
			for (int i = 0; i < triangles.Count; i++)
			{
				ishit = triangles[i].RayCast(ray, epsilon, ref hit) || ishit;
			}
			if (ishit)
				return true;
			return false;
		}
	}
}
