using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public class SunLight : Light
    {
        private const double kEarthSunDistance = 149597870.0 * 1000.0;
        private const double kSunRadius = 6.955 * 100000.0 * 1000.0;
        private const double kEarthRadius = 6371.0 * 1000.0;
        private const double kEarthSurfaceToSun = kEarthSunDistance - kEarthRadius;
        
        private const double kSunArea = 4.0 * Math.PI * kSunRadius * kSunRadius;

        private const float kInvArea = (float)(1.0 / kSunArea);

        public Vector3 sunDirection;

        public Color sunColor;

        public float sunIntensity;

        public bool renderParticipatingMedia;

        //public Vector3 SampleSunDirection(SamplerBase sampler)
        //{
        //    Vector2 diskSp = sampler.SampleUnitDisk() * kR;
        //    Vector3 sp = new Vector3(diskSp.x, diskSp.y, 1.0).normalized;

        //    Vector3 w = sunDirection;
        //    Vector3 u = Vector3.Cross(new Vector3(0.00424f, 1, 0.00764f), w);
        //    u.Normalize();
        //    Vector3 v = Vector3.Cross(u, w);
        //    Vector3 l = sp.x * u + sp.y * v + sp.z * w;
        //    if (Vector3.Dot(l, sunDirection) < 0.0)
        //        l = -sp.x * u - sp.y * v - sp.z * w;
        //    l.Normalize();
        //    return l;
        //}

        //public Color GetSunLightColor()
        //{
        //    return sunIntensity * sunColor;
        //}

        public override Vector3 Sample(SamplerBase sampler, Vector3 hitPoint)
        {
            return hitPoint + (sunDirection * -1.0) * kEarthSurfaceToSun + sampler.SampleSphere() * kSunRadius;
        }

        public override float GetPDF()
        {
            return kInvArea;
        }

        public override Vector3 GetNormal(Vector3 hitPoint, Vector3 surfacePoint)
        {
            return (surfacePoint - (hitPoint + (sunDirection * -1.0) * kEarthSurfaceToSun)).normalized;
        }

        public override float G(Ray ray, Vector3 surfacePoint, Vector3 surfaceNormal)
        {
            double ndl = Vector3.Dot(-1.0 * surfaceNormal, ray.direction);

            double dis2 = (ray.origin - surfacePoint).sqrMagnitude;

            return (float)(ndl / dis2);
        }

        public override bool L(SceneData sceneData, Ray ray, out Color color, out Vector3 surfaceNormal)
        {

            RayCastHit hit = default(RayCastHit);
            hit.distance = double.MaxValue;
            if (sceneData.Raycast(ray, out hit))
            {
                color = Color.black;
                surfaceNormal = default(Vector3);
                return false;
            }

            Vector3 hitNormal;

            if (RayCastSun(ray, out hitNormal))
            {
                double ndl = Vector3.Dot(-1.0 * hitNormal, ray.direction);
                surfaceNormal = hitNormal;
                if (ndl < 0)
                {
                    color = Color.black;
                    return false;
                }
                color = sunIntensity * sunColor;
                return true;
            }

            color = Color.black;
            surfaceNormal = default(Vector3);
            return false;
        }

        private bool RayCastSun(Ray ray, out Vector3 hitNormal)
        {
            Vector3 position = ray.origin + (sunDirection * -1.0) * kEarthSurfaceToSun;
            double radius = kSunRadius;
            Vector3 tocenter = ray.origin - position;
            hitNormal = default(Vector3);

            double vala = Vector3.Dot(ray.direction, ray.direction);
            double valb = Vector3.Dot(tocenter, ray.direction) * 2.0f;
            double valc = Vector3.Dot(tocenter, tocenter) - radius * radius;

            double dis = valb * valb - 4.0f * vala * valc;


            if (dis < 0.0)
                return false;
            else
            {
                double e = Math.Sqrt(dis);
                double denom = 2.0f * vala;
                double t = (-valb - e) / denom;

                if (t > double.Epsilon)
                {
                    hitNormal = (tocenter + ray.direction * t) / radius;

                    return true;
                }

                t = (-valb + e) / denom;

                if (t > double.Epsilon)
                {
                    hitNormal = (tocenter + ray.direction * t) / radius;

                    return true;
                }
            }

            return false;
        }
    }
}
