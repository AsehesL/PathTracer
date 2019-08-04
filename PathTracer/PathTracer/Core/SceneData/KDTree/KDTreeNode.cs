using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    //class KDTreeNode
    //{
    //    private enum AxisType
    //    {
    //        YZ = 0,//x方向
    //        XZ = 1,//y方向
    //        XY = 2,//z方向
    //    }

    //    private List<Geometry> m_Geometries;

    //    private KDTreeNode m_Left;
    //    private KDTreeNode m_Right;

    //    private AxisType m_AxisType;

    //    private double m_Border;

    //    private KDTreeNode() { }

    //    public static KDTreeNode Build(List<Geometry> geometries, Bounds bounds)
    //    {
    //        if (geometries.Count <= 0)
    //            return null;

    //        if (geometries.Count == 1)
    //        {
    //            KDTreeNode lastnode = new KDTreeNode();
    //            lastnode.m_Geometries = geometries;
    //            return lastnode;
    //        }

    //        KDTreeNode node = new KDTreeNode();
    //        if (bounds.size.x >= bounds.size.y && bounds.size.x >= bounds.size.z)
    //            node.m_AxisType = AxisType.YZ;
    //        else if (bounds.size.y >= bounds.size.x && bounds.size.y >= bounds.size.z)
    //            node.m_AxisType = AxisType.XZ;
    //        else if (bounds.size.z >= bounds.size.x && bounds.size.z >= bounds.size.y)
    //            node.m_AxisType = AxisType.XY;

    //        Vector3 mid = Vector3.zero;
    //        float factor = 1.0f / geometries.Count;

    //        for (int i = 0; i < geometries.Count; i++)
    //        {
    //            mid += geometries[i].bounds.center * factor;
    //        }

    //        node.m_Border = mid[(int)node.m_AxisType];

    //        node.m_Geometries = new List<Geometry>();

    //        List<Geometry> leftGeometries = new List<Geometry>();
    //        List<Geometry> rightGeometries = new List<Geometry>();

    //        for (int i = 0; i < geometries.Count; i++)
    //        {
    //            if (geometries[i].bounds.max[(int)node.m_AxisType] < node.m_Border)
    //                leftGeometries.Add(geometries[i]);
    //            else if (geometries[i].bounds.min[(int)node.m_AxisType] > node.m_Border)
    //                rightGeometries.Add(geometries[i]);
    //            else
    //                node.m_Geometries.Add(geometries[i]);
    //        }

    //        Bounds lchildBd = default(Bounds);
    //        Bounds rchildBd = default(Bounds);

    //        double ls = node.m_Border - bounds.min[(int)node.m_AxisType];
    //        double rs = bounds.max[(int)node.m_AxisType] - node.m_Border;

    //        Vector3 lcenter = bounds.center;
    //        Vector3 rcenter = bounds.center;
    //        Vector3 lsize = bounds.size;
    //        Vector3 rsize = bounds.size;
    //        lcenter[(int)node.m_AxisType] = bounds.min[(int)node.m_AxisType] + ls * 0.5f;
    //        rcenter[(int)node.m_AxisType] = bounds.max[(int)node.m_AxisType] - rs * 0.5f;
    //        lsize[(int)node.m_AxisType] = ls;
    //        rsize[(int)node.m_AxisType] = rs;

    //        lchildBd = new Bounds(lcenter, lsize);
    //        rchildBd = new Bounds(rcenter, rsize);

    //        if (leftGeometries.Count > 0)
    //            node.m_Left = Build(leftGeometries, lchildBd);
    //        if (rightGeometries.Count > 0)
    //            node.m_Right = Build(rightGeometries, rchildBd);

    //        return node;

    //    }

    //    public bool Raycast(Ray ray, double epsilon, ref RayCastHit hit)
    //    {
    //        bool raycast = false;
    //        for (int i = 0; i < m_Geometries.Count; i++)
    //        {
    //            if (m_Geometries[i].RayCast(ray, epsilon, ref hit))
    //            {
    //                //hit.g geometry = m_Geometries[i];
    //                raycast = true;
    //            }
    //        }

    //        bool l = ray.origin[(int)m_AxisType] < m_Border;
    //        KDTreeNode first = l ? this.m_Left : this.m_Right;
    //        KDTreeNode last = l ? this.m_Right : this.m_Left;

    //        if (ray.direction[(int)m_AxisType] == 0.0f)
    //        {
    //            if (first != null && first.Raycast(ray, epsilon, ref hit))
    //            {
    //                return true;
    //            }
    //        }
    //        else
    //        {
    //            double dt = (m_Border - ray.origin[(int)m_AxisType]) / ray.direction[(int)m_AxisType];
    //            if (dt >= 0.0f)
    //            {
    //                if (first != null && first.Raycast(ray, epsilon, ref hit))
    //                {
    //                    return true;
    //                }
    //                else if (last != null && last.Raycast(ray, epsilon, ref hit))
    //                {
    //                    return true;
    //                }
    //            }
    //            else
    //            {
    //                if (first != null && first.Raycast(ray, epsilon, ref hit))
    //                {
    //                    return true;
    //                }
    //            }
    //        }

    //        return raycast;
    //    }
    //}
}
