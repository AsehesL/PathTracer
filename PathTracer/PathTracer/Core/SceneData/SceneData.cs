using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public abstract class SceneData
    {
	    private List<Geometry> m_NormalGeometries;

	    public void Build(List<Geometry> geometries)
	    {
		    m_NormalGeometries = new List<Geometry>();
		    List<Triangle> triangles = new List<Triangle>();
		    for (int i = 0; i < geometries.Count; i++)
		    {
			    var triangle = geometries[i] as Triangle;
			    if (triangle != null)
				    triangles.Add(triangle);
			    else
			    {
				    m_NormalGeometries.Add(geometries[i]);
			    }
		    }

		    if (triangles.Count > 0)
		    {
			    BuildForTriangles(triangles);

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

	    protected abstract void BuildForTriangles(List<Triangle> triangles);

	    protected abstract bool RaycastTriangles(Ray ray, double epsilon, ref RayCastHit hit);
    }
}
