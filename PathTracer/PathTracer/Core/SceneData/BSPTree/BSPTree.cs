using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    //class BSPTree : SceneData
    //{
    //    private int m_MaxDepth;

    //    private struct Plane
    //    {
    //        public Vector3 position;
    //        public Vector3 normal;
    //    }

    //    private enum TriangleType
    //    {
    //        CoplanarWithPlane,
    //        InFrontOfPlane,
    //        BehindPlane,
    //        StraddlingPlane,
    //    }

    //    public BSPTree(int depth = 7)
    //    {
    //        m_MaxDepth = depth;
    //    }


    //    protected override void BuildForTriangles(List<Triangle> triangles)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    protected override bool RaycastTriangles(Ray ray, double epsilon, ref RayCastHit hit)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private BSPTreeNode BuildTree(List<Triangle> triangles, int depth)
    //    {
    //        if (triangles == null || triangles.Count == 0)
    //            return null;
    //        if (depth >= m_MaxDepth)
    //            return new BSPTreeNode(triangles);

    //        Plane plane = PickSplittingPlane(triangles);

    //        List<Triangle> frontTriangles = new List<Triangle>();
    //        List<Triangle> backTriangles = new List<Triangle>();

    //        for (int i = 0; i < triangles.Count; i++)
    //        {
    //            TriangleType triangleType = ClassifyTriangleToPlane(triangles[i], plane);
    //            switch (triangleType)
    //            {
    //                case TriangleType.CoplanarWithPlane:
    //                    break;
    //                case TriangleType.InFrontOfPlane:
    //                    break;
    //                case TriangleType.BehindPlane:
    //                    break;
    //                case TriangleType.StraddlingPlane:
    //                    break;
    //            }
    //        }

    //        BSPTreeNode front = BuildTree(frontTriangles, depth + 1);
    //        BSPTreeNode back = BuildTree(backTriangles, depth + 1);

    //        return new BSPTreeNode(front, back);
    //    }

    //    private Plane PickSplittingPlane(List<Triangle> triangles)
    //    {
    //        double K = 0.8;
    //        Plane bestPlane = default(Plane);
    //        float bestScore = float.MaxValue;

    //        for (int i = 0; i < triangles.Count; i++)
    //        {
    //            int numInFront = 0, numBehind = 0, numStradding = 0;
    //            Plane plane = GetPlaneFromTriangle(triangles[i]);
    //        }
    //    }

    //    private TriangleType ClassifyTriangleToPlane(Triangle triangle, Plane plane)
    //    {
            
    //    }

    //    private Plane GetPlaneFromTriangle(Triangle triangle)
    //    {
    //        Vector3 e1 = triangle.vertex1.position - triangle.vertex0.position;
    //        Vector3 e2 = triangle.vertex2.position - triangle.vertex0.position;

    //        Vector3 n = Vector3.Cross(e1, e2).normalized;
    //        return new Plane
    //        {
    //            position = triangle.vertex0.position,
    //            normal = n,
    //        };
    //    }
    //}

    //class BSPTreeNode
    //{
    //}
}
