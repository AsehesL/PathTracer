using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer.SceneSerialization
{
	/// <summary>
	/// Mesh序列化器
	/// </summary>
	[GeometryAnalyse(type = "Mesh")]
	class MeshSerialization : GeometrySerialization
	{
		public string path;

		public Vector3 position;

		public Vector3 euler;

		public Vector3 scale;

		public override void GenerateGeometry(string scenePath, Scene scene, List<Material> materials, List<Geometry> output)
		{
            var fileInfo = new System.IO.FileInfo(scenePath);
            string p = System.IO.Path.Combine(fileInfo.DirectoryName, path);

			var triangles = MeshLoader.LoadMesh(p, position, euler, scale, materials);

			output.AddRange(triangles);
		}
	}
}
