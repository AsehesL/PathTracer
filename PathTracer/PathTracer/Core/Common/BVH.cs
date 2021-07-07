using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public class BVH<T> where T : IAABB
    {
        public BVHNode root { get { return m_Root; } }

        public Bounds bounds
        {
            get
            {
                if (m_Root != null)
                    return m_Root.bounds;
                return default(Bounds);
            }
        }

        private BVHNode m_Root;

        public BVH()
        {
            m_Root = null;
        }

        public void Build(List<T> datas)
        {
            m_Root = null;

            Vector3 min = Vector3.one * double.MaxValue;
            Vector3 max = Vector3.one * double.MinValue;
            List<T> aabbDatas = new List<T>(datas.Count);
            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i] != null)
                {
                    min = Vector3.Min(min, datas[i].GetBounds().min);
                    max = Vector3.Max(max, datas[i].GetBounds().max);

                    aabbDatas.Add(datas[i]);
                }
            }

            if (aabbDatas.Count > 0)
            {
                Bounds bounds = new Bounds((min + max) * 0.5, max - min);
                BuildInternal(aabbDatas, bounds);
            }
        }

        private void BuildInternal(List<T> datas, Bounds bounds)
        {
            List<uint> sortedMortons = new List<uint>();
            for (int i = 0; i < datas.Count; i++)
            {
                //计算所有AABB的morton码
                Vector3 center = datas[i].GetBounds().center;
                double x = (center.x - bounds.min.x) / bounds.size.x;
                double y = (center.y - bounds.min.y) / bounds.size.y;
                double z = (center.z - bounds.min.z) / bounds.size.z;
                uint morton = Morton3D(x, y, z);
                sortedMortons.Add(morton);
            }
            //根据莫顿码对AABB列表排序
            Sort(datas, sortedMortons);

            //生成bvh
            m_Root = GenerateHierarchy(datas, sortedMortons, 0, sortedMortons.Count - 1);
        }

        private BVHNode GenerateHierarchy(List<T> boundsGeometries, List<uint> sortedMortons, int first, int last)
        {
            if (first == last)
                return new BVHNode(boundsGeometries[first]);

            //查找分割平面位置
            int split = FindSplit(sortedMortons, first, last);

            //将AABB列表根据分割平面分割成左右子树
            BVHNode child1 = GenerateHierarchy(boundsGeometries, sortedMortons, first, split);
            BVHNode child2 = GenerateHierarchy(boundsGeometries, sortedMortons, split + 1, last);

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

        private void Sort(List<T> sortedDatas, List<uint> sortedMortons)
        {
            QuickSort(sortedDatas, sortedMortons, 0, sortedMortons.Count - 1);
        }

        private void QuickSort(List<T> sortedDatas, List<uint> sortedMortons, int low, int high)
        {
            int pivot;
            if (low < high)
            {
                pivot = Partition(sortedDatas, sortedMortons, low, high);

                QuickSort(sortedDatas, sortedMortons, low, pivot - 1);
                QuickSort(sortedDatas, sortedMortons, pivot + 1, high);
            }
        }

        private int Partition(List<T> sortedDatas, List<uint> sortedMortons, int low, int high)
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

        private void Swap(List<T> sortedDatas, List<uint> sortedMortons, int a, int b)
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

        public class BVHNode
        {
            public bool isLeaf { get; private set; }

            public BVHNode leftNode { get; private set; }

            public BVHNode rightNode { get; private set; }

            public Bounds bounds { get; private set; }

            public T data { get; private set; }

            public BVHNode(T data)
            {
                this.data = data;
                this.isLeaf = true;
                this.bounds = data.GetBounds();
            }

            public BVHNode(BVHNode leftChild, BVHNode rightChild)
            {
                this.isLeaf = false;
                this.leftNode = leftChild;
                this.rightNode = rightChild;

                Vector3 min = default(Vector3), max = default(Vector3);
                if (leftChild != null)
                {
                    min = leftChild.bounds.min;
                    max = leftChild.bounds.max;
                    if (rightChild != null)
                    {
                        min = Vector3.Min(min, rightChild.bounds.min);
                        max = Vector3.Max(max, rightChild.bounds.max);
                    }
                }
                else if (rightChild != null)
                {
                    min = rightChild.bounds.min;
                    max = rightChild.bounds.max;
                }

                Vector3 si = max - min;
                Vector3 ct = min + si * 0.5;

                if (si.x <= 0)
                    si.x = 0.1;
                if (si.y <= 0)
                    si.y = 0.1;
                if (si.z <= 0)
                    si.z = 0.1;

                this.bounds = new Bounds(ct, si);
            }
        }
    }
}
