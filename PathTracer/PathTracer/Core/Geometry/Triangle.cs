﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
	public struct Vertex
	{
		public Vector3 position;
		public Vector3 normal;
		public Vector3 tangent;
		public Vector2 uv;
	}

	public class Triangle : BoundsGeometry
    {
        public Vertex vertex0;
        public Vertex vertex1;
        public Vertex vertex2;

        public Vertex this[int index]
        {
            get
            {
                if (index == 0)
                    return vertex0;
                if (index == 1)
                    return vertex1;
                if (index == 2)
                    return vertex2;
                throw new System.IndexOutOfRangeException();
            }
        }

        public Triangle(Vertex vertex0, Vertex vertex1, Vertex vertex2, Shader shader) : base(shader)
        {
			this.vertex0 = vertex0;
			this.vertex1 = vertex1;
			this.vertex2 = vertex2;

            double maxX = Math.Max(vertex0.position.x, Math.Max(vertex1.position.x, vertex2.position.x));
            double maxY = Math.Max(vertex0.position.y, Math.Max(vertex1.position.y, vertex2.position.y));
            double maxZ = Math.Max(vertex0.position.z, Math.Max(vertex1.position.z, vertex2.position.z));

            double minX = Math.Min(vertex0.position.x, Math.Min(vertex1.position.x, vertex2.position.x));
            double minY = Math.Min(vertex0.position.y, Math.Min(vertex1.position.y, vertex2.position.y));
            double minZ = Math.Min(vertex0.position.z, Math.Min(vertex1.position.z, vertex2.position.z));

            Vector3 si = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
            Vector3 ct = new Vector3(minX, minY, minZ) + si * 0.5;

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
            double rt = 0.0;

            Vector3 e1 = this.vertex1.position - this.vertex0.position;
            Vector3 e2 = this.vertex2.position - this.vertex0.position;

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
                t = ray.origin - this.vertex0.position;
            }
            else
            {
                t = this.vertex0.position - ray.origin;
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
            hit.texcoord = (1.0 - u - v) * vertex0.uv + u * vertex1.uv + v * vertex2.uv; ;
            hit.normal = (1.0 - u - v) * vertex0.normal + u * vertex1.normal + v * vertex2.normal;
            hit.shader = shader;
            hit.distance = rt;
            return true;
        }
    }
}
