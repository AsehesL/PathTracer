using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
	//class BSPTree : SceneData
	//{
	//	private const int kMaxDepth = 6;
	//	private const int kMinLeafSize = 10;


	//	public override void Build(List<Geometry> geometries)
	//	{
	//		BuildBSPTree(geometries, 0);
	//	}

	//	private BSPNode BuildBSPTree(List<Geometry> geometries, int depth)
	//	{
	//		if (geometries == null || geometries.Count == 0)
	//			return null;
	//		if (depth >= kMaxDepth || geometries.Count <= kMinLeafSize)
	//			return null;
	//		return null;
	//		//return new BSPNode(geometries);
	//	}

	//	public override bool Raycast(Ray ray, double epsilon, out RayCastHit hit)
	//	{
	//		throw new NotImplementedException();
	//	}
	//}
}
