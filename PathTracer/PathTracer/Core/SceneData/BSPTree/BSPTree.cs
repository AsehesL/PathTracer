using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    class BSPTree : SceneData
    {
        private int m_MaxDepth;

        private BSPTreeNode m_Root;

        private struct Plane
        {
            public Vector3 position;
            public Vector3 normal;

            public double d;

            public Plane(Vector3 position, Vector3 normal)
            {
                this.position = position;
                this.normal = normal;
                this.d = Vector3.Dot(position, normal);
            }
        }

        private enum TriangleType
        {
            CoplanarWithPlane,
            InFrontOfPlane,
            BehindPlane,
            StraddlingPlane,
        }

        private enum PointType
        {
            InFrontOfPlane,
            BehindPlane,
            OnPlane,
        }

        public BSPTree(int depth = 7)
        {
            m_MaxDepth = depth;
        }


        protected override void BuildForTriangles(List<Triangle> triangles)
        {
            m_Root = BuildTree(triangles, 0);
        }

        protected override bool RaycastTriangles(Ray ray, double epsilon, ref RayCastHit hit)
        {
            //var node = m_Root;

            //if (node == null)
            //    return false;

            //Stack<BSPTreeNode> nodeStack = new Stack<BSPTreeNode>();
            //Stack<float> timeStack = new Stack<float>();

            //while (true)
            //{
            //    if (!node.IsLeaf)
            //    {
            //        double denom = Vector3.Dot(node.plane.normal, ray.direction);
            //        double dist = node.plane.d - Vector3.Dot(node.plane.normal, ray.origin);
            //        int nearIndex = dist > 0.0 ? 1 : 0;

            //        if (Math.Abs(denom) > double.Epsilon)
            //        {
            //            double t = dist / denom;
            //            if (t >= 0.0)
            //            {
            //                nodeStack.Push(node.);
            //            }
            //        }
            //    }
            //}
            return Raycast(m_Root, ray, epsilon, 0.0, double.MaxValue, ref hit);
        }

        private bool Raycast(BSPTreeNode node, Ray ray, double epsilon, double tMin, double tMax, ref RayCastHit hit)
        {
            if (node == null)
                return false;

            Stack<BSPTreeNode> nodeStack = new Stack<BSPTreeNode>();
            Stack<double> timeStack = new Stack<double>();

            while (true)
            {
                if (!node.IsLeaf)
                {
                    double denom = Vector3.Dot(node.plane.normal, ray.direction);
                    double dist = node.plane.d - Vector3.Dot(node.plane.normal, ray.origin);
                    int nearIndex = dist > 0.0 ? 1 : 0;

                    if (Math.Abs(denom) > double.Epsilon)
                    {
                        double t = dist / denom;
                        if (t >= epsilon && t <= tMax)
                        {
                            if (t >= tMin)
                            {
                                nodeStack.Push(node.childs[1 ^ nearIndex]);
                                timeStack.Push(tMax);
                                tMax = t;
                            }
                            else nearIndex = 1 ^ nearIndex;
                        }
                    }
                    node = node.childs[nearIndex];
                }
                else
                {
                    if (node.triangles != null && node.triangles.Count > 0)
                    {
                        bool ishit = false;
                        for (int i = 0; i < node.triangles.Count; i++)
                        {
                            ishit = node.triangles[i].RayCast(ray, epsilon, ref hit) || ishit;
                        }

                        if (ishit && hit.distance >= tMin && hit.distance <= tMax)
                            return true;
                    }

                    if (nodeStack.Count == 0) break;
                    tMin = tMax;
                    node = nodeStack.Pop();
                    tMax = timeStack.Pop();
                }
            }

            return false;
        }

        private BSPTreeNode BuildTree(List<Triangle> triangles, int depth)
        {
            if (triangles == null || triangles.Count == 0)
                return null;
            if (depth >= m_MaxDepth)
                return new BSPTreeNode(triangles);

            Plane plane = PickSplittingPlane(triangles);

            List<Triangle> frontTriangles = new List<Triangle>();
            List<Triangle> backTriangles = new List<Triangle>();

            for (int i = 0; i < triangles.Count; i++)
            {
                TriangleType triangleType = ClassifyTriangleToPlane(triangles[i], plane);
                switch (triangleType)
                {
                    case TriangleType.CoplanarWithPlane:
                    case TriangleType.InFrontOfPlane:
                        frontTriangles.Add(triangles[i]);
                        break;
                    case TriangleType.BehindPlane:
                        backTriangles.Add(triangles[i]);
                        break;
                    case TriangleType.StraddlingPlane:
                        SplitTriangle(triangles[i], plane, frontTriangles, backTriangles);
                        break;
                }
            }

            BSPTreeNode front = BuildTree(frontTriangles, depth + 1);
            BSPTreeNode back = BuildTree(backTriangles, depth + 1);

            return new BSPTreeNode(front, back, plane);
        }

        private Plane PickSplittingPlane(List<Triangle> triangles)
        {
            double K = 0.8;
            Plane bestPlane = default(Plane);
            double bestScore = double.MaxValue;

            for (int i = 0; i < triangles.Count; i++)
            {
                int numInFront = 0, numBehind = 0, numStradding = 0;
                Plane plane = GetPlaneFromTriangle(triangles[i]);

                for (int j = 0; j < triangles.Count; j++)
                {
                    if (i == j) continue;
                    switch (ClassifyTriangleToPlane(triangles[j], plane))
                    {
                        case TriangleType.CoplanarWithPlane:
                        case TriangleType.InFrontOfPlane:
                            numInFront++;
                            break;
                        case TriangleType.BehindPlane:
                            numBehind++;
                            break;
                        case TriangleType.StraddlingPlane:
                            numStradding++;
                            break;
                    }
                }

                double score = K * numStradding + (1.0 - K) * Math.Abs(numInFront - numBehind);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestPlane = plane;
                }
            }

            return bestPlane;
        }

        private void SplitTriangle(Triangle triangle, Plane plane, List<Triangle> frontTriangles,
            List<Triangle> backTriangles)
        {
            List<Vertex> frontVertices = new List<Vertex>();
            List<Vertex> backVertices = new List<Vertex>();

            SplitEdge(triangle.vertex0, triangle.vertex1, plane, frontVertices, backVertices);
            SplitEdge(triangle.vertex1, triangle.vertex2, plane, frontVertices, backVertices);
            SplitEdge(triangle.vertex2, triangle.vertex0, plane, frontVertices, backVertices);
            if (frontVertices.Count == 4)
            {
                frontTriangles.Add(new Triangle(frontVertices[0], frontVertices[1], frontVertices[2], triangle.shader));
                frontTriangles.Add(new Triangle(frontVertices[0], frontVertices[2], frontVertices[3], triangle.shader));
            }
            else if (frontVertices.Count == 3)
            {
                frontTriangles.Add(new Triangle(frontVertices[0], frontVertices[1], frontVertices[2], triangle.shader));
            }

            if (backVertices.Count == 4)
            {
                backTriangles.Add(new Triangle(backVertices[0], backVertices[1], backVertices[2], triangle.shader));
                backTriangles.Add(new Triangle(backVertices[0], backVertices[2], backVertices[3], triangle.shader));
            }
            else if (backVertices.Count == 3)
            {
                backTriangles.Add(new Triangle(backVertices[0], backVertices[1], backVertices[2], triangle.shader));
            }
        }

        private void SplitEdge(Vertex begin, Vertex end, Plane plane, List<Vertex> frontVertices,
            List<Vertex> behindVertices)
        {
            PointType beginPt = ClassifyPointToPlane(begin.position, plane);
            PointType endPt = ClassifyPointToPlane(end.position, plane);
            double dist = Vector3.Dot(plane.normal, begin.position) - Vector3.Dot(plane.position, plane.normal);
            dist /= Vector3.Distance(begin.position, end.position);

            if ((beginPt == PointType.InFrontOfPlane || beginPt == PointType.OnPlane) && (endPt == PointType.InFrontOfPlane || endPt == PointType.OnPlane))
            {
                frontVertices.Add(begin);
            }
            else if (beginPt == PointType.BehindPlane && endPt == PointType.BehindPlane)
            {
                behindVertices.Add(begin);
            }
            else if ((beginPt == PointType.InFrontOfPlane || beginPt == PointType.OnPlane) &&
                     endPt == PointType.BehindPlane)
            {
                frontVertices.Add(begin);
                frontVertices.Add(Vertex.Lerp(begin, end, dist));
                behindVertices.Add(Vertex.Lerp(begin, end, dist));
            }
            else if (beginPt == PointType.BehindPlane &&
                     (endPt == PointType.InFrontOfPlane || endPt == PointType.OnPlane))
            {
                behindVertices.Add(begin);
                behindVertices.Add(Vertex.Lerp(begin, end, dist));
                frontVertices.Add(Vertex.Lerp(begin, end, dist));
            }
        }

        private TriangleType ClassifyTriangleToPlane(Triangle triangle, Plane plane)
        {
            int numInFront = 0, numBehind = 0;
            for (int i = 0; i < 3; i++)
            {
                Vertex v = triangle[i];
                switch (ClassifyPointToPlane(v.position, plane))
                {
                    case PointType.InFrontOfPlane:
                        numInFront++;
                        break;
                    case PointType.BehindPlane:
                        numBehind++;
                        break;
                }
            }

            if (numBehind != 0 && numInFront != 0)
                return TriangleType.StraddlingPlane;
            if (numInFront != 0)
                return TriangleType.InFrontOfPlane;
            if (numBehind != 0)
                return TriangleType.BehindPlane;
            return TriangleType.CoplanarWithPlane;

        }

        private PointType ClassifyPointToPlane(Vector3 point, Plane plane)
        {
            double dist = Vector3.Dot(plane.normal, point) - plane.d;
            if (dist > 0.000001)
                return PointType.InFrontOfPlane;
            if (dist < 0.000001)
                return PointType.BehindPlane;
            return PointType.OnPlane;
        }

        private Plane GetPlaneFromTriangle(Triangle triangle)
        {
            Vector3 e1 = triangle.vertex1.position - triangle.vertex0.position;
            Vector3 e2 = triangle.vertex2.position - triangle.vertex0.position;

            Vector3 n = Vector3.Cross(e1, e2).normalized;
            return new Plane(triangle.vertex0.position, n);
        }

        class BSPTreeNode
        {
            public List<Triangle> triangles;
            public BSPTreeNode[] childs;
            public Plane plane;
            public bool IsLeaf
            {
                get { return m_IsLeaf; }
            }

            private bool m_IsLeaf;

            public BSPTreeNode(List<Triangle> triangles)
            {
                this.triangles = triangles;
                m_IsLeaf = true;
            }

            public BSPTreeNode(BSPTreeNode front, BSPTreeNode back, Plane plane)
            {
                this.childs = new BSPTreeNode[] {front, back};
                this.plane = plane;
                m_IsLeaf = false;
            }
        }
    }
}
