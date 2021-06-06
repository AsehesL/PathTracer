using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public abstract class SceneData
    {
		public abstract void Build(List<Geometry> geometries);

		public abstract Bounds GetBounds();

	    public bool Raycast(Ray ray, out RayCastHit hit)
	    {
		    hit = default(RayCastHit);
		    hit.distance = double.MaxValue;
			
			return RaycastTriangles(ray, ref hit);
		}

	    protected abstract bool RaycastTriangles(Ray ray, ref RayCastHit hit);
    }
}
