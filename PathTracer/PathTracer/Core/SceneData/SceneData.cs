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
		    Vector3 min = Vector3.one * double.MaxValue;
		    Vector3 max = Vector3.one * double.MinValue;
			for (int i = 0; i < geometries.Count; i++)
		    {
			    var triangle = geometries[i] as Triangle;
			    if (triangle != null)
			    {
				    max = Vector3.Max(triangle.vertex0.position, max);
				    max = Vector3.Max(triangle.vertex1.position, max);
				    max = Vector3.Max(triangle.vertex2.position, max);
				    min = Vector3.Min(triangle.vertex0.position, min);
				    min = Vector3.Min(triangle.vertex1.position, min);
				    min = Vector3.Min(triangle.vertex2.position, min);
					triangles.Add(triangle);
			    }
			    else
			    {
				    m_NormalGeometries.Add(geometries[i]);
			    }
		    }

		    if (triangles.Count > 0)
		    {
			    Bounds bounds = new Bounds((min + max) * 0.5, max - min);
			    BuildForTriangles(triangles, bounds);

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

	    protected abstract void BuildForTriangles(List<Triangle> triangles, Bounds bounds);

	    protected abstract bool RaycastTriangles(Ray ray, double epsilon, ref RayCastHit hit);
    }
}
