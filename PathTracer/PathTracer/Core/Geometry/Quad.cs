using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public class Quad : Geometry
    {
        public double widthSquared;
        public double heightSquared;
        public Vector3 right;
        public Vector3 up;
        public Vector3 position;
        public Vector3 normal;

        private float m_InvArea;

        public Quad(Vector3 position, Vector3 normal, Vector3 right, Vector3 up, Material material) : base(material)
        {
            this.widthSquared = right.sqrMagnitude;
            this.heightSquared = up.sqrMagnitude;
            this.right = right;
            this.up = up;
            this.position = position;
            this.normal = normal.normalized;

            m_InvArea = 1.0f / ((float)Math.Sqrt(widthSquared * heightSquared));

            Vector3 pos0 = position;
            Vector3 pos1 = position + right;
            Vector3 pos2 = position + up;
            Vector3 pos3 = position + right + up;

            Vector3 min = pos0;
            Vector3 max = pos0;

            min = Vector3.Min(min, pos1);
            min = Vector3.Min(min, pos2);
            min = Vector3.Min(min, pos3);

            max = Vector3.Max(max, pos1);
            max = Vector3.Max(max, pos2);
            max = Vector3.Max(max, pos3);

            Vector3 si = max - min;
            Vector3 ct = min + si * 0.5;

            if (si.x <= 0)
                si.x = 0.1;
            if (si.y <= 0)
                si.y = 0.1;
            if (si.z <= 0)
                si.z = 0.1;

            m_bounds = new Bounds(ct, si);
        }

        protected override bool RayCastGeometry(Ray ray, ref RayCastHit hit)
        {
            double t = Vector3.Dot(this.position - ray.origin, this.normal) / Vector3.Dot(ray.direction, this.normal);
            if (t <= double.Epsilon)
                return false;
            if (t > hit.distance)
                return false;
            Vector3 p = ray.origin + ray.direction * t;
            Vector3 d = p - this.position;
            double ddw = Vector3.Dot(d, this.right);
            if (ddw < 0.0 || ddw > this.widthSquared)
                return false;
            double ddh = Vector3.Dot(d, this.up);
            if (ddh < 0.0 || ddh > this.heightSquared)
                return false;
            hit.distance = t;
            hit.normal = normal;

	        Vector3 lp = p - this.position;
	        double texcoordx = Vector3.Dot(lp, this.right) / this.right.magnitude;
	        double texcoordy = Vector3.Dot(lp, this.up) / this.up.magnitude;

            hit.texcoord = new Vector2(texcoordx, texcoordy);
            hit.normal = this.normal;
            Vector3 tang = this.right.normalized;
            hit.tangent = new Vector4(tang.x, tang.y, tang.z, 1.0);
            hit.material = material;
            hit.geometry = this;
            hit.hit = p;
            double ndv = Vector3.Dot(normal, ray.origin - hit.hit);
            if (ndv < 0)
            {
                if (material != null && !material.ShouldRenderBackFace())
                    return false;
                hit.isBackFace = true;
                hit.normal *= -1;
            }
            else
                hit.isBackFace = false;

            //if (hit.isBackFace)
            //    hit.hit -= hit.normal * 0.00000000000001;
            //else
                hit.hit += hit.normal * 0.00000000000001;
            //hit.distance = Vector3.Distance(hit.hit, ray.origin);

            return true;
        }

        public override float GetPDF()
        {
            return m_InvArea;
        }

        public override Vector3 Sample(SamplerBase sampler)
        {
            Vector2 pos = sampler.SampleUnitDisk();

            return position + right * (pos.x) + up * (pos.y);
        }

        public override Vector3 GetNormal(Vector3 point)
        {
            return normal;
        }
    }
}
