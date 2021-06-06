using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public class AreaLight : Light
    {
        private Geometry m_Geometry;

        public AreaLight(Geometry geometry)
        {
            m_Geometry = geometry;
        }

        public override Vector3 Sample(SamplerBase sampler, Vector3 hitPoint)
        {
            if (m_Geometry == null)
                return Vector3.zero;
            return m_Geometry.Sample(sampler);
        }

        public override Vector3 GetNormal(Vector3 hitPoint, Vector3 surfacePoint)
        {
            if (m_Geometry == null)
                return Vector3.zero;
            return m_Geometry.GetNormal(surfacePoint);
        }

        public override float GetPDF()
        {
            if (m_Geometry == null)
                return 1.0f;
            return m_Geometry.GetPDF();
        }

        public override bool L(SceneData sceneData, Ray ray, out Color color, out Vector3 normal)
        {
            if (m_Geometry == null || m_Geometry.material == null)
            {
                color = Color.black;
                normal = default(Vector3);
                return false;
            }
            RayCastHit hit = default(RayCastHit);
            hit.distance = double.MaxValue;
            if (sceneData.Raycast(ray, out hit))
            {
                if (hit.geometry == m_Geometry && hit.material == m_Geometry.material)
                {
                    double ndl = Vector3.Dot(-1.0 * hit.normal, ray.direction);
                    normal = hit.normal;
                    if (ndl < 0)
                    {
                        color = Color.black;
                        return false;
                    }
                    color = m_Geometry.material.GetEmissive(hit);
                    return true;
                }
            }

            color = Color.black;
            normal = default(Vector3);
            return false;
        }

        public override float G(Ray ray, Vector3 samplePoint, Vector3 normal)
        {
            double ndl = Vector3.Dot(-1.0 * normal, ray.direction);

            double dis2 = (ray.origin - samplePoint).sqrMagnitude;

            return (float)(ndl / dis2);
        }
    }
}
