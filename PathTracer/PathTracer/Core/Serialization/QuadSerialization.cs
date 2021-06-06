using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer.SceneSerialization
{
	[GeometryAnalyse(type = "Quad")]
	class QuadSerialization : GeometrySerialization
	{
		public Vector3 position;

		public Vector3 normal;

		public Vector3 right;

		public Vector3 up;

        public override void GenerateGeometry(string scenePath, Scene scene, List<Material> materials, List<Geometry> output)
		{
			output.Add(new Quad(position, normal, right, up, materials.Count > 0 ? materials[0] : null));
		}
	}
}
