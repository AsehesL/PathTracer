using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
	class Cube : Geometry
	{
		public Vector3 position;
		public Vector3 scale;

		private bool m_IsRotated;

		private Matrix m_LocalToWorld;
		private Matrix m_WorldToLocal;

		private double m_Area0;
		private double m_Area1;
		private double m_Area2;

		private double m_InvArea;

		public Cube(Vector3 position, Vector3 scale, Vector3 euler, Material material) : base(material)
		{
			this.position = position;
			this.scale = scale;

			m_Area0 = scale.x * scale.y;
			m_Area1 = scale.y * scale.z;
			m_Area2 = scale.x * scale.z;

			m_InvArea = 1.0 / (m_Area0 * 2.0 + m_Area1 * 2.0 + m_Area2 * 2.0);

			Matrix localToWorld = Matrix.TRS(position, euler, Vector3.one);
			Vector3 p0 = new Vector3(-scale.x * 0.5, -scale.y * 0.5, -scale.z * 0.5);
			Vector3 p1 = new Vector3(-scale.x * 0.5, -scale.y * 0.5, scale.z * 0.5);
			Vector3 p2 = new Vector3(scale.x * 0.5, -scale.y * 0.5, -scale.z * 0.5);
			Vector3 p3 = new Vector3(scale.x * 0.5, -scale.y * 0.5, scale.z * 0.5);
			Vector3 p4 = new Vector3(-scale.x * 0.5, scale.y * 0.5, -scale.z * 0.5);
			Vector3 p5 = new Vector3(-scale.x * 0.5, scale.y * 0.5, scale.z * 0.5);
			Vector3 p6 = new Vector3(scale.x * 0.5, scale.y * 0.5, -scale.z * 0.5);
			Vector3 p7 = new Vector3(scale.x * 0.5, scale.y * 0.5, scale.z * 0.5);

			p0 = localToWorld.TransformPoint(p0);
			p1 = localToWorld.TransformPoint(p1);
			p2 = localToWorld.TransformPoint(p2);
			p3 = localToWorld.TransformPoint(p3);
			p4 = localToWorld.TransformPoint(p4);
			p5 = localToWorld.TransformPoint(p5);
			p6 = localToWorld.TransformPoint(p6);
			p7 = localToWorld.TransformPoint(p7);

			Vector3 min = p0;
			Vector3 max = p0;

			min = Vector3.Min(min, p1);
			min = Vector3.Min(min, p2);
			min = Vector3.Min(min, p3);
			min = Vector3.Min(min, p4);
			min = Vector3.Min(min, p5);
			min = Vector3.Min(min, p6);
			min = Vector3.Min(min, p7);

			max = Vector3.Max(max, p1);
			max = Vector3.Max(max, p2);
			max = Vector3.Max(max, p3);
			max = Vector3.Max(max, p4);
			max = Vector3.Max(max, p5);
			max = Vector3.Max(max, p6);
			max = Vector3.Max(max, p7);

			this.m_bounds = Bounds.CreateByMinAndMax(min, max);

			m_IsRotated = false;
			if (euler.IsZero() == false)
				m_IsRotated = true;

			m_LocalToWorld = localToWorld;
			m_WorldToLocal = localToWorld.inverse;
		}

		protected override bool RayCastGeometry(Ray ray, ref RayCastHit hit)
		{
			double tmin = 0.0;
			double tmax = hit.distance;

			Vector3 cubeCenter = m_IsRotated ? Vector3.zero : position;
			Vector3 min = cubeCenter - scale * 0.5;
			Vector3 max = cubeCenter + scale * 0.5;
			Vector3 normal = default(Vector3);

			Vector3 rayOrigin = m_IsRotated ? m_WorldToLocal.TransformPoint(ray.origin) : ray.origin;
			Vector3 rayDir = m_IsRotated ? m_WorldToLocal.TransformVector(ray.direction) : ray.direction;

			int hitFace = 0;

			for (int i = 0; i < 3; i++)
			{
			    Vector3 n = new Vector3(i == 0 ? -1 : 0, i == 1 ? -1 : 0, i == 2 ? -1 : 0);
                if (Math.Abs(rayDir[i]) < double.Epsilon)
				{
					if (rayOrigin[i] < min[i] || rayOrigin[i] > max[i])
						return false;
				}
				else
				{
					int hf = i + 1;

					double ood = 1.0 / rayDir[i];
					double t1 = (min[i] - rayOrigin[i]) * ood;
					double t2 = (max[i] - rayOrigin[i]) * ood;
					
					if (t1 > t2)
					{
						double t = t2;
						t2 = t1;
						t1 = t;
						n *= -1;

						hf = i + 4;
					}

					if (t1 > tmin)
					{
						tmin = t1;

						normal = n;
						hitFace = hf;
					}

					if (t2 < tmax)
					{
						tmax = t2;
					}

					if (tmin > tmax)
						return false;
				}
			}

		    //if (tmin > -double.Epsilon && tmin < double.Epsilon)
		    //    return false;
			hit.distance = tmin;
			hit.material = material;
			hit.geometry = this;
		    hit.hit = rayOrigin + rayDir * hit.distance;
			Vector3 tangent = Vector3.zero;
			if (hitFace == 1)
			{
				hit.texcoord = new Vector2((hit.hit.z - cubeCenter.z) / (scale.z * 0.5f) * 0.5f + 0.5f, (hit.hit.y - cubeCenter.y) / (scale.y * 0.5f) * 0.5f + 0.5f);
				tangent = new Vector3(0, 0, 1);
			}
			else if (hitFace == 2)
            {
                hit.texcoord = new Vector2(-(hit.hit.x - cubeCenter.x) / (scale.x * 0.5f) * 0.5f - 0.5f, (hit.hit.z - cubeCenter.z) / (scale.z * 0.5f) * 0.5f + 0.5f);
				tangent = new Vector3(-1, 0, 0);
			}
            else if (hitFace == 3)
            {
                hit.texcoord = new Vector2(-(hit.hit.x - cubeCenter.x) / (scale.x * 0.5f) * 0.5f - 0.5f, (hit.hit.y - cubeCenter.y) / (scale.y * 0.5f) * 0.5f + 0.5f);
				tangent = new Vector3(-1, 0, 0);
			}
			else if (hitFace == 4)
            {
                hit.texcoord = new Vector2(-(hit.hit.z - cubeCenter.z) / (scale.z * 0.5f) * 0.5f - 0.5f, (hit.hit.y - cubeCenter.y) / (scale.y * 0.5f) * 0.5f + 0.5f);
				tangent = new Vector3(0, 0, -1);
			}
            else if (hitFace == 5)
            {
                hit.texcoord = new Vector2((hit.hit.x - cubeCenter.x) / (scale.x * 0.5f) * 0.5f + 0.5f, (hit.hit.z - cubeCenter.z) / (scale.z * 0.5f) * 0.5f + 0.5f);
				tangent = new Vector3(1, 0, 0);
			}
            else if (hitFace == 6)
            {
                hit.texcoord = new Vector2((hit.hit.x - cubeCenter.x) / (scale.x * 0.5f) * 0.5f + 0.5f, (hit.hit.y - cubeCenter.y) / (scale.y * 0.5f) * 0.5f + 0.5f);
				tangent = new Vector3(1, 0, 0);
			}
			hit.normal = normal;
			double ndv = Vector3.Dot(normal, rayOrigin - hit.hit);
			if (ndv < 0)
			{
                if (material != null && !material.ShouldRenderBackFace())
                    return false;
                hit.isBackFace = true;
				//hit.normal *= -1;
			}
			else
				hit.isBackFace = false;

			if (hit.isBackFace)
				hit.hit -= hit.normal * 0.00000000000001;
			else
				hit.hit += hit.normal * 0.00000000000001;
			//hit.distance = Vector3.Distance(hit.hit, ray.origin);

			if (m_IsRotated)
			{
				hit.hit = m_LocalToWorld.TransformPoint(hit.hit);
				hit.normal = m_LocalToWorld.TransformVector(hit.normal);
				tangent = m_LocalToWorld.TransformVector(tangent);

			}
			hit.tangent = new Vector4(tangent.x, tangent.y, tangent.z, 1);

			return true;
		}

		public override Vector3 GetNormal(Vector3 point)
		{
			if (m_IsRotated)
			{
				Vector3 p = m_WorldToLocal.TransformPoint(point);
				Vector3 normal = GetLocalNormal(p);
				return m_LocalToWorld.TransformVector(normal);
			}
			return GetLocalNormal(point - position);
		}

		public override Vector3 Sample(SamplerBase sampler)
		{
			Vector3 localPoint = SampleLocalPosition(sampler);

			if (m_IsRotated)
			{
				return m_LocalToWorld.TransformPoint(localPoint);
			}
			else
			{
				return position + localPoint;
			}
		}

		public override float GetPDF()
		{
			return (float)m_InvArea;
		}

		private Vector3 SampleLocalPosition(SamplerBase sampler)
		{
            double rand = sampler.GetRandom();

            double areaRand = rand * (m_Area0 * 2.0 + m_Area1 * 2.0 + m_Area2 * 2.0);

            Vector2 sample = sampler.SampleUnitSquare();
            if (areaRand < m_Area0)
            {
                return new Vector3(-scale.x * 0.5 + scale.x * sample.x, -scale.y * 0.5 + scale.y * sample.y, -scale.z * 0.5);
            }
            else if (areaRand >= m_Area0 && areaRand < m_Area0 * 2.0)
            {
                return new Vector3(-scale.x * 0.5 + scale.x * sample.x, -scale.y * 0.5 + scale.y * sample.y, scale.z * 0.5);
            }
            else if (areaRand >= m_Area0 * 2.0 && areaRand < m_Area0 * 2.0 + m_Area1)
            {
                return new Vector3(-scale.x * 0.5, -scale.y * 0.5 + scale.y * sample.x, -scale.z * 0.5 + scale.z * sample.y);
            }
            else if (areaRand >= m_Area0 * 2.0 + m_Area1 && areaRand < m_Area0 * 2.0 + m_Area1 * 2.0)
            {
                return new Vector3(scale.x * 0.5, -scale.y * 0.5 + scale.y * sample.x, -scale.z * 0.5 + scale.z * sample.y);
            }
            else if (areaRand >= m_Area0 * 2.0 + m_Area1 * 2.0 && areaRand < m_Area0 * 2.0 + m_Area1 * 2.0 + m_Area2)
            {
                return new Vector3(-scale.x * 0.5 + scale.x * sample.x, -scale.y * 0.5, -scale.z * 0.5 + scale.z * sample.y);
            }
            else
            {
                return new Vector3(-scale.x * 0.5 + scale.x * sample.x, scale.y * 0.5, -scale.z * 0.5 + scale.z * sample.y);
            }
        }

		private Vector3 GetLocalNormal(Vector3 localPoint)
		{
            if (localPoint.x <= scale.x * 0.5f && localPoint.x >= -scale.x * 0.5f && localPoint.z <= scale.z * 0.5f && localPoint.z >= -scale.z * 0.5f)
            {
                if (localPoint.y > 0)
                    return new Vector3(0, 1, 0);
                else
                    return new Vector3(0, -1, 0);
            }
            if (localPoint.x <= scale.x * 0.5f && localPoint.x >= -scale.x * 0.5f && localPoint.y <= scale.y * 0.5f && localPoint.y >= -scale.y * 0.5f)
            {
                if (localPoint.z > 0)
                    return new Vector3(0, 0, 1);
                else
                    return new Vector3(0, 0, -1);
            }
            if (localPoint.z <= scale.z * 0.5f && localPoint.z >= -scale.z * 0.5f && localPoint.y <= scale.y * 0.5f && localPoint.y >= -scale.y * 0.5f)
            {
                if (localPoint.x > 0)
                    return new Vector3(1, 0, 0);
                else
                    return new Vector3(-1, 0, 0);
            }
            return new Vector3();
        }
	}
}
