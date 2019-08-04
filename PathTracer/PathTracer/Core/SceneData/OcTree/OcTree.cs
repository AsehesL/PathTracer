using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    //class OcTree : SceneData
    //{
    //    private OcTreeNode m_Node;

    //    public override void Build(List<Geometry> geometries)
    //    {
    //        Bounds bounds = geometries[0].bounds;
    //        for (int i = 1; i < geometries.Count; i++)
    //        {
    //            bounds.Encapsulate(geometries[i].bounds);
    //        }

    //        bounds.size *= 1.1;

    //        m_Node = new OcTreeNode(bounds);

    //        for (int i = 0; i < geometries.Count; i++)
    //        {
    //            m_Node.Insert(geometries[i], 0, 5);
    //        }
    //    }

    //    public override bool Raycast(Ray ray, double epsilon, out RayCastHit hit)
    //    {
    //        hit = default(RayCastHit);
    //        hit.distance = double.MaxValue;
    //        return RaycastInOcTree(ray, epsilon, m_Node, true, ref hit);
    //    }

    //    private static bool RaycastInOcTree(Ray ray, double epsilon, OcTreeNode node, bool testBounds,
    //        ref RayCastHit hit)
    //    {
    //        if (node == null)
    //            return false;
    //        bool raycast = false;
    //        if (!testBounds || node.bounds.Raycast(ray))
    //        {
    //            for (int i = 0; i < node.geometries.Count; i++)
    //            {
    //                Geometry geometry = node.geometries[i];

    //                if (geometry.RayCast(ray, epsilon, ref hit))
    //                {
    //                    raycast = true;
    //                }
    //            }

    //            int code = 0;
    //            code = RaycastYZ(ray, node.bounds.center, node.bounds.size.z, node.bounds.size.y, code);
    //            code = RaycastXY(ray, node.bounds.center, node.bounds.size.x, node.bounds.size.y, code);
    //            code = RaycastXZ(ray, node.bounds.center, node.bounds.size.x, node.bounds.size.z, code);
                
                
    //            if (code == 0)
    //                return false;
    //            for (int i = 0; i < 8; i++)
    //            {
    //                var child = node.GetChild(i);
    //                if (child != null)
    //                {
    //                    int c = 1 << (i + 1);
    //                    if ((code & c) != 0)
    //                    {
    //                        if (RaycastInOcTree(ray, epsilon, child, false, ref hit))
    //                        {
    //                            raycast = true;
    //                        }
    //                    }
    //                }
    //            }

    //            //bool flb = false;
    //            //bool flt = false;
    //            //bool frb = false;
    //            //bool frt = false;
    //            //bool blb = false;
    //            //bool blt = false;
    //            //bool brb = false;
    //            //bool brt = false;

    //                  //bool xRight = ray.origin.x >= node.bounds.center.x;
    //                    //bool xLeft = ray.origin.x <= node.bounds.center.x;
    //                    //bool yTop = ray.origin.y >= node.bounds.center.y;
    //                    //bool yBottom = ray.origin.y <= node.bounds.center.y;
    //                    //bool zForward = ray.origin.z >= node.bounds.center.z;
    //                    //bool zBack = ray.origin.z <= node.bounds.center.z;

    //                    //int yzPos = RaycastRect(ray, node.bounds.center, Vector3.right, Vector3.forward, Vector3.up, node.bounds.size.z, node.bounds.size.y);
    //                    //int xyPos = RaycastRect(ray, node.bounds.center, Vector3.forward, Vector3.right, Vector3.up, node.bounds.size.x, node.bounds.size.y);
    //                    //int xzPos = RaycastRect(ray, node.bounds.center, Vector3.up, Vector3.right, Vector3.forward, node.bounds.size.x, node.bounds.size.z);

    //                    //flb = (xLeft && yBottom && zForward) || yzPos == 3 || xyPos == 0 || xzPos == 1;
    //                    //flt = (xLeft && yTop && zForward) || yzPos == 2 || xyPos == 1 || xzPos == 1;
    //                    //frb = (xRight && yBottom && zForward) || yzPos == 3 || xyPos == 3 || xzPos == 2;
    //                    //frt = (xRight && yTop && zForward) || yzPos == 2 || xyPos == 2 || xzPos == 2;
    //                    //blb = (xLeft && yBottom && zBack) || yzPos == 0 || xyPos == 0 || xzPos == 0;
    //                    //blt = (xLeft && yTop && zBack) || yzPos == 1 || xyPos == 1 || xzPos == 0;
    //                    //brb = (xRight && yBottom && zBack) || yzPos == 0 || xyPos == 3 || xzPos == 3;
    //                    //brt = (xRight && yTop && zBack) || yzPos == 1 || xyPos == 2 || xzPos == 3;

    //                    //if (flb)
    //                    //{
    //                    //    if (RaycastInOcTree(ray, epsilon, node.GetChild(0), false, ref hit))
    //                    //    {
    //                    //        raycast = true;
    //                    //    }
    //                    //}
    //                    //if (flt)
    //                    //{
    //                    //    if (RaycastInOcTree(ray, epsilon, node.GetChild(1), false, ref hit))
    //                    //    {
    //                    //        raycast = true;
    //                    //    }
    //                    //}
    //                    //if (frb)
    //                    //{
    //                    //    if (RaycastInOcTree(ray, epsilon, node.GetChild(2), false, ref hit))
    //                    //    {
    //                    //        raycast = true;
    //                    //    }
    //                    //}
    //                    //if (frt)
    //                    //{
    //                    //    if (RaycastInOcTree(ray, epsilon, node.GetChild(3), false, ref hit))
    //                    //    {
    //                    //        raycast = true;
    //                    //    }
    //                    //}
    //                    //if (blb)
    //                    //{
    //                    //    if (RaycastInOcTree(ray, epsilon, node.GetChild(4), false, ref hit))
    //                    //    {
    //                    //        raycast = true;
    //                    //    }
    //                    //}
    //                    //if (blt)
    //                    //{
    //                    //    if (RaycastInOcTree(ray, epsilon, node.GetChild(5), false, ref hit))
    //                    //    {
    //                    //        raycast = true;
    //                    //    }
    //                    //}
    //                    //if (brb)
    //                    //{
    //                    //    if (RaycastInOcTree(ray, epsilon, node.GetChild(6), false, ref hit))
    //                    //    {
    //                    //        raycast = true;
    //                    //    }
    //                    //}
    //                    //if (brt)
    //                    //{
    //                    //    if (RaycastInOcTree(ray, epsilon, node.GetChild(7), false, ref hit))
    //                    //    {
    //                    //        raycast = true;
    //                    //    }
    //                    //}




    //                    ////int nearB = -1;
    //                    ////double dis = double.MaxValue;
    //                    //for (int i = 0; i < 8; i++)
    //                    //{
    //                    //    var child = node.GetChild(i);
    //                    //    if (child != null)
    //                    //    {
    //                    //        //double currentDis = 0.0;
    //                    //        //if (child.bounds.Raycast(ray, out currentDis))
    //                    //        //{
    //                    //        //    if (currentDis < dis)
    //                    //        //    {
    //                    //        //        dis = currentDis;
    //                    //        //        nearB = i;
    //                    //        //    }
    //                    //        //}
    //                    //        if (RaycastInOcTree(ray, epsilon, child, true, ref hit))
    //                    //        {
    //                    //            raycast = true;
    //                    //        }
    //                    //    }
    //                    //}
    //            }

    //        return raycast;

    //    }

    //    private static int RaycastXZ(Ray ray, Vector3 position, double width, double height, int code)
    //    {
    //        double t = (position.y - ray.origin.y) / ray.direction.y;
    //        if (t <= 0.0f)
    //            return code;

    //        Vector3 p = ray.origin + t * ray.direction;
    //        Vector3 d = p - position;

    //        double ddw = d.x;
    //        double ddh = d.z;

    //        if (ddw >= -width * 0.5 && ddw < 0 && ddh <= height * 0.5 && ddh > 0)
    //        {
    //            code |= 1;
    //            code |= 1<<2;
    //        }
    //        else if (ddw <= width * 0.5 && ddw > 0 && ddh <= height * 0.5 && ddh > 0)
    //        {
    //            code |= 1<<3;
    //            code |= 1 << 4;
    //        }
    //        else if (ddw >= -width * 0.5 && ddw < 0 && ddh >= -height * 0.5 && ddh < 0)
    //        {
    //            code |= 1 << 5;
    //            code |= 1 << 6;
    //        }
    //        else if (ddw <= width * 0.5 && ddw > 0 && ddh >= -height * 0.5 && ddh < 0)
    //        {
    //            code |= 1 << 7;
    //            code |= 1 << 8;
    //        }

    //        return code;
    //    }

    //    private static int RaycastXY(Ray ray, Vector3 position, double width, double height, int code)
    //    {
    //        double t = (position.z - ray.origin.z) / ray.direction.z;
    //        if (t <= 0.0f)
    //            return code;

    //        Vector3 p = ray.origin + t * ray.direction;
    //        Vector3 d = p - position;

    //        double ddw = d.x;
    //        double ddh = d.y;

    //        if (ddw >= -width * 0.5 && ddw < 0 && ddh <= height * 0.5 && ddh > 0)
    //        {
    //            code |= 1 << 2;
    //            code |= 1 << 6;
    //        }
    //        else if (ddw <= width * 0.5 && ddw > 0 && ddh <= height * 0.5 && ddh > 0)
    //        {
    //            code |= 1 << 4;
    //            code |= 1 << 8;
    //        }
    //        else if (ddw >= -width * 0.5 && ddw < 0 && ddh >= -height * 0.5 && ddh < 0)
    //        {
    //            code |= 1 << 1;
    //            code |= 1 << 5;
    //        }
    //        else if (ddw <= width * 0.5 && ddw > 0 && ddh >= -height * 0.5 && ddh < 0)
    //        {
    //            code |= 1 << 3;
    //            code |= 1 << 7;
    //        }

    //        return code;
    //    }

    //    private static int RaycastYZ(Ray ray, Vector3 position, double width, double height, int code)
    //    {
    //        double t = (position.x - ray.origin.x) / ray.direction.x;
    //        if (t <= 0.0f)
    //            return code;

    //        Vector3 p = ray.origin + t * ray.direction;
    //        Vector3 d = p - position;

    //        double ddw = d.z;
    //        double ddh = d.y;

    //        if (ddw >= -width * 0.5 && ddw < 0 && ddh <= height * 0.5 && ddh > 0)
    //        {
    //            code |= 1 << 6;
    //            code |= 1 << 8;
    //        }
    //        else if (ddw <= width * 0.5 && ddw > 0 && ddh <= height * 0.5 && ddh > 0)
    //        {
    //            code |= 1 << 2;
    //            code |= 1 << 4;
    //        }
    //        else if (ddw >= -width * 0.5 && ddw < 0 && ddh >= -height * 0.5 && ddh < 0)
    //        {
    //            code |= 1 << 5;
    //            code |= 1 << 7;
    //        }
    //        else if (ddw <= width * 0.5 && ddw > 0 && ddh >= -height * 0.5 && ddh < 0)
    //        {
    //            code |= 1 << 1;
    //            code |= 1 << 3;
    //        }

    //        return code;
    //    }

    //    //private static int RaycastRect(Ray ray, Vector3 position, Vector3 normal, Vector3 right, Vector3 up, double width,
    //    //    double height)
    //    //{
    //    //    int pos = -1;
    //    //    double t = Vector3.Dot(position - ray.origin, normal) /
    //    //              Vector3.Dot(ray.direction, normal);

    //    //    if (t <= 0.0f)
    //    //        return -1;

    //    //    Vector3 p = ray.origin + t * ray.direction;
    //    //    Vector3 d = p - position;

    //    //    double ddw = Vector3.Dot(d, right);
    //    //    double ddh = Vector3.Dot(d, up);

    //    //    if (ddw >= -width * 0.5 && ddw <= 0 && ddh >= -height * 0.5 && ddh <= 0)
    //    //        pos = 0;
    //    //    else if (ddw >= -width * 0.5 && ddw <= 0 && ddh <= height * 0.5 && ddh > 0)
    //    //        pos = 1;
    //    //    else if (ddw <= width * 0.5 && ddw >= 0 && ddh <= height * 0.5 && ddh > 0)
    //    //        pos = 2;
    //    //    else if (ddw <= width * 0.5 && ddw >= 0 && ddh >= -height * 0.5 && ddh <= 0)
    //    //        pos = 3;

    //    //    return pos;
    //    //}
    //}
}
