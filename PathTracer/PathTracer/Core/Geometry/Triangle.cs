using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    class Triangle : Geometry
    {
        public Vector3 v0;
        public Vector3 v1;
        public Vector3 v2;

        public Vector3 n0;
        public Vector3 n1;
        public Vector3 n2;

        public Vector2 uv0;
        public Vector2 uv1;
        public Vector2 uv2;

        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 n0, Vector3 n1, Vector3 n2, Vector2 uv0, Vector2 uv1, Vector2 uv2, Shader shader) : base(shader)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            this.n0 = n0;
            this.n1 = n1;
            this.n2 = n2;
            this.uv0 = uv0;
            this.uv1 = uv1;
            this.uv2 = uv2;

            double maxX = Math.Max(v0.x, Math.Max(v1.x, v2.x));
            double maxY = Math.Max(v0.y, Math.Max(v1.y, v2.y));
            double maxZ = Math.Max(v0.z, Math.Max(v1.z, v2.z));

            double minX = Math.Min(v0.x, Math.Min(v1.x, v2.x));
            double minY = Math.Min(v0.y, Math.Min(v1.y, v2.y));
            double minZ = Math.Min(v0.z, Math.Min(v1.z, v2.z));

            Vector3 si = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
            Vector3 ct = new Vector3(minX, minY, minZ) + si / 2;

            if (si.x <= 0)
                si.x = 0.1;
            if (si.y <= 0)
                si.y = 0.1;
            if (si.z <= 0)
                si.z = 0.1;

            this.bounds = new Bounds(ct, si);
        }

        public override bool RayCast(Ray ray, double epsilon, ref RayCastHit hit)
        {
            if (bounds.Raycast(ray) == false)
                return false;
            double rt = 0.0f;

            Vector3 e1 = this.v1 - this.v0;
            Vector3 e2 = this.v2 - this.v0;

            double v = 0;
            double u = 0;

            Vector3 n = Vector3.Cross(e1, e2);
            double ndv = Vector3.Dot(ray.direction, n);
            if (ndv > 0)
            {
                return false;
            }

            Vector3 p = Vector3.Cross(ray.direction, e2);

            double det = Vector3.Dot(e1, p);
            Vector3 t = default(Vector3);
            if (det > 0)
            {
                t = ray.origin - this.v0;
            }
            else
            {
                t = this.v0 - ray.origin;
                det = -det;
            }
            if (det < epsilon)
            {
                return false;
            }

            u = Vector3.Dot(t, p);
            if (u < 0.0 || u > det)
                return false;

            Vector3 q = Vector3.Cross(t, e1);

            v = Vector3.Dot(ray.direction, q);
            if (v < 0.0 || u + v > det)
                return false;

            rt = Vector3.Dot(e2, q);

            double finvdet = 1.0 / det;
            rt *= finvdet;
            if (rt < 0.001)
                return false;
            if (rt > hit.distance)
                return false;
            u *= finvdet;
            v *= finvdet;

            hit.hit = ray.origin + ray.direction * rt;
            hit.texcoord = (1.0f - u - v) * uv0 + u * uv1 + v * uv2; ;
            hit.normal = (1.0f - u - v) * n0 + u * n1 + v * n2;
            hit.shader = shader;
            hit.distance = rt;
            return true;
        }
    }

    //class Mesh
    //{
    //    private List<Triangle> m_Triangles;

    //    public Mesh(Vector3 position, Vector3 euler, Vector3 scale, string path, Shader shader)
    //    {
    //        Matrix matrix = Matrix.TRS(position, euler, scale);

    //        m_Triangles = MeshLoader.LoadMesh(path, matrix, shader);

    //    }

    //    //public override bool RayCast(Ray ray, ref RayCastHit hit)
    //    //{
    //    //    if (m_Triangles == null)
    //    //        return false;

    //    //    bool result = false;
    //    //    for (int i = 0; i < m_Triangles.Length; i ++)
    //    //    {
    //    //        if (RaycastTriangle(ray, m_Triangles[i], ref hit))
    //    //            result = true;
    //    //    }
    //    //    return result;
    //    //}

        
        
    //}
}
