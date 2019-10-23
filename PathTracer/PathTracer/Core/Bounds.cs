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
	        distance = 0.0;
	        double tmax = double.MaxValue;

	        for (int i = 0; i < 3; i++)
	        {
		        if (Math.Abs(ray.direction[i]) < double.Epsilon)
		        {
			        if (ray.origin[i] < min[i] || ray.origin[i] > max[i])
				        return false;
		        }
		        else
		        {
			        double ood = 1.0 / ray.direction[i];
			        double t1 = (min[i] - ray.origin[i]) * ood;
			        double t2 = (max[i] - ray.origin[i]) * ood;
			        if (t1 > t2)
			        {
				        double t = t2;
				        t2 = t1;
				        t1 = t;
			        }

			        if (t1 > distance)
			        {
				        distance = t1;
			        }

			        if (t2 < tmax)
			        {
				        tmax = t2;
			        }

			        if (distance > tmax)
				        return false;
		        }
	        }
	        return true;
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
