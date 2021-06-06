using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer.SceneSerialization
{
	[GeometryAnalyse(type = "Sphere")]
	class SphereSerialization : GeometrySerialization
	{
		public Vector3 position;

		public float radius;

		public override void GenerateGeometry(string scenePath, Scene scene, List<Material> materials, List<Geometry> output)
		{
			output.Add(new Sphere(position, radius, materials.Count > 0 ? materials[0] : null));
		}
	}
}
