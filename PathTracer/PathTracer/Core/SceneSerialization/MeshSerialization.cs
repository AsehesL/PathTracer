using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathTracer.Core.Utils;

namespace ASL.PathTracer.SceneSerialization
{
	[GeometryAnalyse(type = "Mesh")]
	class MeshSerialization : GeometrySerialization
	{
		public override void GenerateGeometry(Shader shader, string scenePath, List<Geometry> output, Dictionary<string, GeometryParamData> geoParams)
		{
			string path = geoParams["Path"].paramValue;
			string euler = geoParams["Euler"].paramValue;
			string scale = geoParams["Scale"].paramValue;
			string position = geoParams["Position"].paramValue;

			var fileInfo = new System.IO.FileInfo(scenePath);
			string p = System.IO.Path.Combine(fileInfo.DirectoryName, path);

			Vector3 rot = StringUtils.StringToVector3(euler);
			Vector3 sca = StringUtils.StringToVector3(scale);
			Vector3 pos = StringUtils.StringToVector3(position);

			var triangles = MeshLoader.LoadMesh(p, pos, rot, sca, shader);

			output.AddRange(triangles);
		}
	}
}
