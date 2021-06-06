using System;
using System.Collections.Generic;

namespace ASL.PathTracer
{
	/// <summary>
	/// 层次包围盒
	/// </summary>
	class BVHSceneData : SceneData
	{
		private BVH<Geometry> m_bvh;

		public override Bounds GetBounds()
		{
			return m_bvh != null ? m_bvh.bounds : default(Bounds);
		}

		public override void Build(List<Geometry> geometries)
		{
            m_bvh = new BVH<Geometry>();
            m_bvh.Build(geometries);
        }

	    protected override bool RaycastTriangles(Ray ray, ref RayCastHit hit)
		{
			bool raycast = false;
			if (m_bvh == null || m_bvh.root == null)
				return raycast;
			if (m_bvh.root.isLeaf)
			{
				if (m_bvh.root.data != null)
					return m_bvh.root.data.RayCast(ray, ref hit);

				return raycast;
			}

			Stack<BVH<Geometry>.BVHNode> nodes = new Stack<BVH<Geometry>.BVHNode>();
			if (m_bvh.root.bounds.Raycast(ray))
			{
				nodes.Push(m_bvh.root);
			}

			while (nodes.Count > 0)
			{
				var node = nodes.Pop();
				if (node.isLeaf == false)
				{
					double hitleftdistance = 0.0, hitrightdistance = 0.0;
					bool hitleft = node.leftNode != null ? node.leftNode.bounds.Raycast(ray, out hitleftdistance) : false;
					bool hitright = node.rightNode != null ? node.rightNode.bounds.Raycast(ray, out hitrightdistance) : false;
					if (hitleft && hitright)
					{
						if (hitleftdistance < hitrightdistance)
						{
							nodes.Push(node.rightNode);
							nodes.Push(node.leftNode);
						}
						else
						{
							nodes.Push(node.leftNode);
							nodes.Push(node.rightNode);
						}
					}
					else if (hitleft)
						nodes.Push(node.leftNode);
					else if (hitright)
						nodes.Push(node.rightNode);
				}
				else
				{
					if (node.data != null)
					{
						raycast = node.data.RayCast(ray, ref hit) || raycast;
					}
				}
			}
			return raycast;
		}
	}
}
