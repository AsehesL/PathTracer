using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
	//class BSPTree : SceneData
	//{
	//	private int m_MaxDepth;

	//	private struct Plane
	//	{
	//		public Vector3 position;
	//		public Vector3 normal;
	//	}

	//	private enum TriangleType
	//	{
	//		CoplanarWithPlane,
	//		InFrontOfPlane,
	//		BehindPlane,
	//		StraddlingPlane,
	//	}

	//	public BSPTree(int depth = 7)
	//	{
	//		m_MaxDepth = depth;
	//	}


	//	protected override void BuildForTriangles(List<Triangle> triangles)
	//	{
	//		throw new NotImplementedException();
	//	}

	//	protected override bool RaycastTriangles(Ray ray, double epsilon, ref RayCastHit hit)
	//	{
	//		throw new NotImplementedException();
	//	}

	//	private BSPTreeNode BuildTree(List<Triangle> triangles, int depth)
	//	{
	//		if (triangles == null || triangles.Count == 0)
	//			return null;
	//		if (depth >= m_MaxDepth)
	//			return new BSPTreeNode(triangles);

	//		Plane plane = PickSplittingPlane(triangles);

	//		List<Triangle> frontTriangles = new List<Triangle>();
	//		List<Triangle> backTriangles = new List<Triangle>();

	//		for (int i = 0; i < triangles.Count; i++)
	//		{
	//			TriangleType triangleType = ClassifyTriangleToPlane(triangles[i], plane);
	//			switch (triangleType)
	//			{
	//				case TriangleType.CoplanarWithPlane:
	//					break;
	//				case TriangleType.InFrontOfPlane:
	//					break;
	//				case TriangleType.BehindPlane:
	//					break;
	//				case TriangleType.StraddlingPlane:
	//					break;
	//			}
	//		}

	//		BSPTreeNode front = BuildTree(frontTriangles, depth + 1);
	//		BSPTreeNode back = BuildTree(backTriangles, depth + 1);

	//		return new BSPTreeNode(front, back);
	//	}

	//	private Plane PickSplittingPlane(List<Triangle> triangles)
	//	{

	//	}

	//	private TriangleType ClassifyTriangleToPlane(Triangle triangle, Plane plane)
	//	{

	//	}
	//}

	//class BSPTreeNode
	//{
	//}
}
