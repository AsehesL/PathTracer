using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    //class OcTreeNode
    //{
    //    public Bounds bounds;

    //    public List<Geometry> geometries
    //    {
    //        get { return m_Geometries; }
    //    }

    //    public OcTreeNode GetChild(int index)
    //    {
    //        if (index == 0)
    //            return m_ForwardLeftBottom;
    //        else if (index == 1)
    //            return m_ForwardLeftTop;
    //        else if (index == 2)
    //            return m_ForwardRightBottom;
    //        else if (index == 3)
    //            return m_ForwardRightTop;
    //        else if (index == 4)
    //            return m_BackLeftBottom;
    //        else if (index == 5)
    //            return m_BackLeftTop;
    //        else if (index == 6)
    //            return m_BackRightBottom;
    //        else if (index == 7)
    //            return m_BackRightTop;
    //        return null;
    //    }

    //    private OcTreeNode m_ForwardLeftTop;
    //    private OcTreeNode m_ForwardLeftBottom;
    //    private OcTreeNode m_ForwardRightTop;
    //    private OcTreeNode m_ForwardRightBottom;
    //    private OcTreeNode m_BackLeftTop;
    //    private OcTreeNode m_BackLeftBottom;
    //    private OcTreeNode m_BackRightTop;
    //    private OcTreeNode m_BackRightBottom;

    //    private List<Geometry> m_Geometries;

    //    public OcTreeNode(Bounds bounds)
    //    {
    //        this.bounds = bounds;
    //        this.m_Geometries = new List<Geometry>();
    //    }

    //    public OcTreeNode Insert(Geometry geometry, int depth, int maxDepth)
    //    {
    //        if (depth < maxDepth)
    //        {
    //            OcTreeNode node = GetContainerNode(geometry);
    //            if (node != null)
    //                return node.Insert(geometry, depth + 1, maxDepth);
    //        }
    //        m_Geometries.Add(geometry);
    //        return this;
    //    }

    //    private OcTreeNode GetContainerNode(Geometry geometry)
    //    {
    //        Vector3 halfSize = bounds.size * 0.5;
    //        OcTreeNode result = null;
    //        result = GetContainerNode(ref m_ForwardLeftBottom,
    //            bounds.center + new Vector3(-halfSize.x * 0.5, -halfSize.y * 0.5, halfSize.z * 0.5),
    //            halfSize, geometry);
    //        if (result != null)
    //            return result;

    //        result = GetContainerNode(ref m_ForwardLeftTop,
    //            bounds.center + new Vector3(-halfSize.x * 0.5, halfSize.y * 0.5, halfSize.z * 0.5),
    //            halfSize, geometry);
    //        if (result != null)
    //            return result;

    //        result = GetContainerNode(ref m_ForwardRightBottom,
    //            bounds.center + new Vector3(halfSize.x * 0.5, -halfSize.y * 0.5, halfSize.z * 0.5),
    //            halfSize, geometry);
    //        if (result != null)
    //            return result;

    //        result = GetContainerNode(ref m_ForwardRightTop,
    //            bounds.center + new Vector3(halfSize.x * 0.5, halfSize.y * 0.5, halfSize.z * 0.5),
    //            halfSize, geometry);
    //        if (result != null)
    //            return result;

    //        result = GetContainerNode(ref m_BackLeftBottom,
    //            bounds.center + new Vector3(-halfSize.x * 0.5, -halfSize.y * 0.5, -halfSize.z * 0.5),
    //            halfSize, geometry);
    //        if (result != null)
    //            return result;

    //        result = GetContainerNode(ref m_BackLeftTop,
    //            bounds.center + new Vector3(-halfSize.x * 0.5, halfSize.y * 0.5, -halfSize.z * 0.5),
    //            halfSize, geometry);
    //        if (result != null)
    //            return result;

    //        result = GetContainerNode(ref m_BackRightBottom,
    //            bounds.center + new Vector3(halfSize.x * 0.5, -halfSize.y * 0.5, -halfSize.z * 0.5),
    //            halfSize, geometry);
    //        if (result != null)
    //            return result;

    //        result = GetContainerNode(ref m_BackRightTop,
    //            bounds.center + new Vector3(halfSize.x * 0.5, halfSize.y * 0.5, -halfSize.z * 0.5),
    //            halfSize, geometry);
    //        if (result != null)
    //            return result;

    //        return null;
    //    }

    //    private OcTreeNode GetContainerNode(ref OcTreeNode node, Vector3 centerPos, Vector3 size, Geometry geometry)
    //    {
    //        OcTreeNode result = null;
    //        Bounds bd = geometry.bounds;
    //        if (node == null)
    //        {
    //            Bounds bounds = new Bounds(centerPos, size);
    //            if (bounds.Contains(bd))
    //            {
    //                node = new OcTreeNode(bounds);
    //                result = node;
    //            }
    //        }
    //        else if (node.bounds.Contains(bd))
    //        {
    //            result = node;
    //        }
    //        return result;
    //    }
    //}
}
