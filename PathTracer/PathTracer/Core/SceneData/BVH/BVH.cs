using System;
using System.Collections.Generic;

namespace ASL.PathTracer
{
	class BVH : SceneData
	{
		private BVHNode m_Root;

		protected override void BuildForTriangles(List<Triangle> triangles, Bounds bounds)
		{
			List<uint> sortedMortons = new List<uint>();
			for (int i = 0; i < triangles.Count; i++)
			{
				Vector3 center = triangles[i].bounds.center;
				double x = (center.x - bounds.min.x) / bounds.size.x;
				double y = (center.y - bounds.min.y) / bounds.size.y;
				double z = (center.z - bounds.min.z) / bounds.size.z;
				uint morton = Morton3D(x, y, z);
				sortedMortons.Add(morton);
			}
			Sort(triangles, sortedMortons);

			m_Root = GenerateHierarchy(triangles, sortedMortons, 0, sortedMortons.Count - 1);
		}

		protected override bool RaycastTriangles(Ray ray, double epsilon, ref RayCastHit hit)
		{
			bool raycast = false;
			if (m_Root.IsLeaf)
			{
				if (m_Root.Triangle != null)
					return m_Root.Triangle.RayCast(ray, epsilon, ref hit);

				return raycast;
			}

			Stack<BVHNode> nodes = new Stack<BVHNode>();
			if (m_Root.Bounds.Raycast(ray))
			{
				nodes.Push(m_Root);
			}

			while (nodes.Count > 0)
			{
				var node = nodes.Pop();
				if (node.IsLeaf == false)
				{
					double hitleftdistance = 0.0, hitrightdistance = 0.0;
					bool hitleft = node.LeftNode != null ? node.LeftNode.Bounds.Raycast(ray, out hitleftdistance) : false;
					bool hitright = node.RightNode != null ? node.RightNode.Bounds.Raycast(ray, out hitrightdistance) : false;
					if (hitleft && hitright)
					{
						if (hitleftdistance < hitrightdistance)
						{
							nodes.Push(node.RightNode);
							nodes.Push(node.LeftNode);
						}
						else
						{
							nodes.Push(node.LeftNode);
							nodes.Push(node.RightNode);
						}
					}
					else if (hitleft)
						nodes.Push(node.LeftNode);
					else if (hitright)
						nodes.Push(node.RightNode);
				}
				else
				{
					if (node.Triangle != null)
					{
						raycast = node.Triangle.RayCast(ray, epsilon, ref hit) || raycast;
					}
				}
			}
			return raycast;
		}

		private BVHNode GenerateHierarchy(List<Triangle> sortedTriangles, List<uint> sortedMortons, int first, int last)
		{
			if (first == last)
				return new BVHNode(sortedTriangles[first]);

			int split = FindSplit(sortedMortons, first, last);

			BVHNode child1 = GenerateHierarchy(sortedTriangles, sortedMortons, first, split);
			BVHNode child2 = GenerateHierarchy(sortedTriangles, sortedMortons, split + 1, last);

			return new BVHNode(child1, child2);
		}

		private int FindSplit(List<uint> sortedMortons, int first, int last)
		{
			uint firstCode = sortedMortons[first];
			uint lastCode = sortedMortons[last];

			if (firstCode == lastCode)
				return (first + last) >> 1;

			int commonPrefix = CountLeadingZeros(firstCode ^ lastCode);

			int split = first;
			int step = last - first;

			do
			{
				step = (step + 1) >> 1;
				int newSplit = split + step;

				if (newSplit < last)
				{
					uint splitCode = sortedMortons[newSplit];
					int splitPrefix = CountLeadingZeros(firstCode ^ splitCode);
					if (splitPrefix > commonPrefix)
						split = newSplit;
				}
			}
			while (step > 1);

			return split;
		}

		private int CountLeadingZeros(uint i)
		{
			int ret = 0;
			uint temp = ~i;

			while ((temp & 0x80000000) > 0)
			{
				temp <<= 1;
				ret++;
			}
			return ret;
		}

		private void Sort(List<Triangle> sortedDatas, List<uint> sortedMortons)
		{
			QuickSort(sortedDatas, sortedMortons, 0, sortedMortons.Count - 1);
		}

		private void QuickSort(List<Triangle> sortedDatas, List<uint> sortedMortons, int low, int high)
		{
			int pivot;
			if (low < high)
			{
				pivot = Partition(sortedDatas, sortedMortons, low, high);

				QuickSort(sortedDatas, sortedMortons, low, pivot - 1);
				QuickSort(sortedDatas, sortedMortons, pivot + 1, high);
			}
		}

		private int Partition(List<Triangle> sortedDatas, List<uint> sortedMortons, int low, int high)
		{
			uint pivotkey = sortedMortons[low];
			while (low < high)
			{
				while (low < high && sortedMortons[high] >= pivotkey)
					high--;
				Swap(sortedDatas, sortedMortons, low, high);
				while (low < high && sortedMortons[low] <= pivotkey)
					low++;
				Swap(sortedDatas, sortedMortons, low, high);
			}
			return low;
		}

		private void Swap(List<Triangle> sortedDatas, List<uint> sortedMortons, int a, int b)
		{
			var tempData = sortedDatas[a];
			uint tempMorton = sortedMortons[a];
			sortedDatas[a] = sortedDatas[b];
			sortedDatas[b] = tempData;
			sortedMortons[a] = sortedMortons[b];
			sortedMortons[b] = tempMorton;
		}

		private uint ExpandBits(uint v)
		{
			v = (v * 0x00010001u) & 0xFF0000FFu;
			v = (v * 0x00000101u) & 0x0F00F00Fu;
			v = (v * 0x00000011u) & 0xC30C30C3u;
			v = (v * 0x00000005u) & 0x49249249u;
			return v;
		}

		/// <summary>
		/// 计算莫顿码
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		private uint Morton3D(double x, double y, double z)
		{
			x = Math.Min(Math.Max(x * 1024.0f, 0.0f), 1023.0f);
			y = Math.Min(Math.Max(y * 1024.0f, 0.0f), 1023.0f);
			z = Math.Min(Math.Max(z * 1024.0f, 0.0f), 1023.0f);
			uint xx = ExpandBits((uint)x);
			uint yy = ExpandBits((uint)y);
			uint zz = ExpandBits((uint)z);
			return xx * 4 + yy * 2 + zz;
		}

		class BVHNode
		{
			public bool IsLeaf { get; private set; }

			public BVHNode LeftNode { get; private set; }

			public BVHNode RightNode { get; private set; }

			public Bounds Bounds { get; private set; }

			public Triangle Triangle { get; private set; }

			public BVHNode(Triangle triangle)
			{
				this.Triangle = triangle;
				this.IsLeaf = true;
				this.Bounds = triangle.bounds;
			}

			public BVHNode(BVHNode leftChild, BVHNode rightChild)
			{
				this.IsLeaf = false;
				this.LeftNode = leftChild;
				this.RightNode = rightChild;

				Vector3 min = default(Vector3), max = default(Vector3);
				if (leftChild != null)
				{
					min = leftChild.Bounds.min;
					max = leftChild.Bounds.max;
					if (rightChild != null)
					{
						min = Vector3.Min(min, rightChild.Bounds.min);
						max = Vector3.Max(max, rightChild.Bounds.max);
					}
				}
				else if (rightChild != null)
				{
					min = rightChild.Bounds.min;
					max = rightChild.Bounds.max;
				}

				Vector3 si = max - min;
				Vector3 ct = min + si * 0.5;

				if (si.x <= 0)
					si.x = 0.1;
				if (si.y <= 0)
					si.y = 0.1;
				if (si.z <= 0)
					si.z = 0.1;

				this.Bounds = new Bounds(ct, si);
			}
		}
	}
}
