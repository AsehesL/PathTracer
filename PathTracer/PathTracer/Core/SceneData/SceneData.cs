using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public abstract class SceneData
    {
		public Sky sky;

		private List<Geometry> m_NormalGeometries;

	    public void Build(List<Geometry> geometries)
	    {
		    m_NormalGeometries = new List<Geometry>();
		    List<BoundsGeometry> boundsGeometries = new List<BoundsGeometry>();
		    Vector3 min = Vector3.one * double.MaxValue;
		    Vector3 max = Vector3.one * double.MinValue;
			for (int i = 0; i < geometries.Count; i++)
		    {
			    var boundsGeometry = geometries[i] as BoundsGeometry;
			    if (boundsGeometry != null)
			    {
				    min = Vector3.Min(min, boundsGeometry.bounds.min);
				    max = Vector3.Max(max, boundsGeometry.bounds.max);

			        boundsGeometries.Add(boundsGeometry);
			    }
			    else
			    {
				    m_NormalGeometries.Add(geometries[i]);
			    }
		    }

		    if (boundsGeometries.Count > 0)
		    {
			    Bounds bounds = new Bounds((min + max) * 0.5, max - min);
			    BuildForBoundsGeometries(boundsGeometries, bounds);

		    }
	    }

	    public bool Raycast(Ray ray, double epsilon, out RayCastHit hit)
	    {
		    hit = default(RayCastHit);
		    hit.distance = double.MaxValue;
		    bool result = false;
		    for (int i = 0; i < m_NormalGeometries.Count; i++)
		    {
			    if (m_NormalGeometries[i].RayCast(ray, epsilon, ref hit))
			    {
				    result = true;
			    }
		    }

		    result = result || RaycastTriangles(ray, epsilon, ref hit);
			
			return result;
		}

	    protected abstract void BuildForBoundsGeometries(List<BoundsGeometry> boundsGeometries, Bounds bounds);

	    protected abstract bool RaycastTriangles(Ray ray, double epsilon, ref RayCastHit hit);
    }
}
