using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public struct Bounds
    {
        public Vector3 center;
        public Vector3 size;

        public Vector3 max
        {
            get { return new Vector3(center.x + size.x * 0.5, center.y + size.y * 0.5, center.z + size.z * 0.5); }
        }

        public Vector3 min
        {
            get { return new Vector3(center.x - size.x * 0.5, center.y - size.y * 0.5, center.z - size.z * 0.5); }
        }

        public Bounds(Vector3 center, Vector3 size)
        {
            this.center = center;
            this.size = size;
        }

        public void Encapsulate(Bounds bounds)
        {
            Vector3 max = Vector3.Max(bounds.max, this.max);
            Vector3 min = Vector3.Min(bounds.min, this.min);

            Vector3 si = new Vector3(max.x - min.x, max.y - min.y, max.z - min.z);
            Vector3 ct = new Vector3(min.x, min.y, min.z) + si *0.5;

            if (si.x <= 0)
                si.x = 0.1;
            if (si.y <= 0)
                si.y = 0.1;
            if (si.z <= 0)
                si.z = 0.1;

            this.center = ct;
            this.size = si;
        }

        public bool Raycast(Ray ray)
        {
            double dis = 0.0;
            return Raycast(ray, out dis);
        }

        public bool Raycast(Ray ray, out double distance)
        {
            //distance = 0;
            //double tmax = double.MaxValue;

            ////if (ray.origin.x > min.x && ray.origin.y > min.y && ray.origin.z > min.z && ray.origin.x < max.x && ray.origin.y < max.y && ray.origin.z < max.z)
            ////{
            ////    return true;
            ////}

            //for (int i = 0; i < 3; i++)
            //{
            //    if (ray.direction[i] < double.Epsilon && ray.direction[i] > -double.Epsilon)
            //    {
            //        if (ray.origin[i] < min[i] || ray.origin[i] > max[i]) return false;
            //    }
            //    else
            //    {
            //        double ood = 1.0 / ray.direction[i];
            //        double t1 = (min[i] - ray.origin[i]) * ood;
            //        double t2 = (max[i] - ray.origin[i]) * ood;
            //        if (t1 > t2)
            //        {
            //            double t = t1;
            //            t1 = t2;
            //            t2 = t;
            //        }

            //        if (t1 > distance) distance = t1;
            //        if (t2 < tmax) tmax = t2; 
            //        if (distance > tmax) return false;
            //    }
            //}

            //return true;


            distance = 0.0;
            double t;
            bool hit = false;
            Vector3 hitpoint;
            Vector3 min = this.min;
            Vector3 max = this.max;
            Vector3 rayorig = ray.origin;
            Vector3 raydir = ray.direction;

            if (rayorig.x > min.x && rayorig.y > min.y && rayorig.z > min.z && rayorig.x < max.x && rayorig.y < max.y && rayorig.z < max.z)
            {
                return true;
            }

            if (rayorig.x < min.x && raydir.x > 0)
            {
                t = (min.x - rayorig.x) / raydir.x;

                if (t > 0)
                {
                    hitpoint = rayorig + raydir * t;
                    if (hitpoint.y >= min.y && hitpoint.y <= max.y && hitpoint.z >= min.z && hitpoint.z <= max.z && (!hit || t < distance))
                    {
                        hit = true;
                        distance = t;
                    }
                }
            }

            if (rayorig.x > max.x && raydir.x < 0)
            {
                t = (max.x - rayorig.x) / raydir.x;
                if (t > 0)
                {
                    hitpoint = rayorig + raydir * t;
                    if (hitpoint.y > min.y && hitpoint.y <= max.y &&
                     hitpoint.z >= min.z && hitpoint.z <= max.z &&
                     (!hit || t < distance))
                    {
                        hit = true;
                        distance = t;
                    }
                }
            }

            if (rayorig.y < min.y && raydir.y > 0)
            {
                t = (min.y - rayorig.y) / raydir.y;
                if (t > 0)
                {
                    hitpoint = rayorig + raydir * t;
                    if (hitpoint.x >= min.x && hitpoint.x <= max.x &&
                     hitpoint.z >= min.z && hitpoint.z <= max.z &&
                     (!hit || t < distance))
                    {
                        hit = true;
                        distance = t;
                    }
                }
            }

            if (rayorig.y > max.y && raydir.y < 0)
            {
                t = (max.y - rayorig.y) / raydir.y;
                if (t > 0)
                {
                    hitpoint = rayorig + raydir * t;
                    if (hitpoint.x >= min.x && hitpoint.x <= max.x &&
                     hitpoint.z >= min.z && hitpoint.z <= max.z &&
                     (!hit || t < distance))
                    {
                        hit = true;
                        distance = t;
                    }
                }
            }

            if (rayorig.z < min.z && raydir.z > 0)
            {
                t = (min.z - rayorig.z) / raydir.z;
                if (t > 0)
                {
                    hitpoint = rayorig + raydir * t;
                    if (hitpoint.x >= min.x && hitpoint.x <= max.x &&
                     hitpoint.y >= min.y && hitpoint.y <= max.y &&
                     (!hit || t < distance))
                    {
                        hit = true;
                        distance = t;
                    }
                }
            }

            if (rayorig.z > max.z && raydir.z < 0)
            {
                t = (max.z - rayorig.z) / raydir.z;
                if (t > 0)
                {
                    hitpoint = rayorig + raydir * t;
                    if (hitpoint.x >= min.x && hitpoint.x <= max.x &&
                     hitpoint.y >= min.y && hitpoint.y <= max.y &&
                     (!hit || t < distance))
                    {
                        hit = true;
                        distance = t;
                    }
                }
            }

            return hit;
        }

        public bool Contains(Vector3 point)
        {
            if (point.x <= min.x || point.x >= max.x)
                return false;
            if (point.y <= min.y || point.y >= max.y)
                return false;
            if (point.z <= min.z || point.z >= max.z)
                return false;
            return true;
        }

        public bool Contains(Bounds compareTo)
        {
            if (!Contains(compareTo.center + new Vector3(-compareTo.size.x * 0.5, compareTo.size.y * 0.5, -compareTo.size.z * 0.5)))
                return false;
            if (!Contains(compareTo.center + new Vector3(compareTo.size.x * 0.5, compareTo.size.y * 0.5, -compareTo.size.z * 0.5)))
                return false;
            if (!Contains(compareTo.center + new Vector3(compareTo.size.x * 0.5, compareTo.size.y * 0.5, compareTo.size.z * 0.5)))
                return false;
            if (!Contains(compareTo.center + new Vector3(-compareTo.size.x * 0.5, compareTo.size.y * 0.5, compareTo.size.z * 0.5)))
                return false;
            if (!Contains(compareTo.center + new Vector3(-compareTo.size.x * 0.5, -compareTo.size.y * 0.5, -compareTo.size.z * 0.5)))
                return false;
            if (!Contains(compareTo.center + new Vector3(compareTo.size.x * 0.5, -compareTo.size.y * 0.5, -compareTo.size.z * 0.5)))
                return false;
            if (!Contains(compareTo.center + new Vector3(compareTo.size.x * 0.5, -compareTo.size.y * 0.5, compareTo.size.z * 0.5)))
                return false;
            if (!Contains(compareTo.center + new Vector3(-compareTo.size.x * 0.5, -compareTo.size.y * 0.5, compareTo.size.z * 0.5)))
                return false;
            return true;
        }
    }
}
