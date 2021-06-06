using System.Collections;
using System.Collections.Generic;
using ASL.PathTracer.SceneSerialization;

namespace ASL.PathTracer.SceneSerialization
{
	/// <summary>
	/// Cube序列化器
	/// </summary>
	[GeometryAnalyse(type = "Cube")]
	class CubeSerialization : GeometrySerialization
	{
		public Vector3 position;
		public Vector3 scale;
		public Vector3 euler;

		public override void GenerateGeometry(string scenePath, Scene scene, List<Material> materials, List<Geometry> output)
        {
            output.Add(new Cube(position, scale, euler, materials.Count > 0 ? materials[0] : null));
        }
	}
}