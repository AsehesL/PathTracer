using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
	class Cube : BoundsGeometry
	{
		public Vector3 position;
		public Vector3 scale;

		public Cube(Vector3 position, Vector3 scale, Shader shader) : base(shader)
		{
			this.position = position;
			this.scale = scale;

			this.bounds = new Bounds(position, scale + new Vector3(0.1, 0.1, 0.1));
		}

		protected override bool RayCastGeometry(Ray ray, double epsilon, ref RayCastHit hit)
		{
			double tmin = 0.0;
			double tmax = hit.distance;

			Vector3 min = position - scale * 0.5;
			Vector3 max = position + scale * 0.5;
			Vector3 normal = default(Vector3);

			for (int i = 0; i < 3; i++)
			{
			    Vector3 n = new Vector3(i == 0 ? -1 : 0, i == 1 ? -1 : 0, i == 2 ? -1 : 0);
                if (Math.Abs(ray.direction[i]) < epsilon)
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
						n *= -1;
					}

					if (t1 > tmin)
					{
						tmin = t1;
						normal = n;
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
			hit.distance = tmin - epsilon * 2;
			hit.shader = shader;
		    hit.hit = ray.origin + ray.direction * hit.distance;
			hit.texcoord = default(Vector2); //Cube射线检测uv，懒得实现 o(*￣3￣)o 
			hit.normal = normal;
			return true;
		}
	}
}
