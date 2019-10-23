using System.Collections;
using System.Collections.Generic;
using ASL.PathTracer.SceneSerialization;
using PathTracer.Core.Utils;

namespace ASL.PathTracer.SceneSerialization
{
	[GeometryAnalyse(type = "Cube")]
	class CubeSerialization : GeometrySerialization
	{
		public override void GenerateGeometry(Shader shader, string scenePath, List<Geometry> output, Dictionary<string, GeometryParamData> geoParams)
		{
			string position = geoParams["Position"].paramValue;
			string scale = geoParams["Scale"].paramValue;
			Vector3 s = StringUtils.StringToVector3(scale);
			Vector3 p = StringUtils.StringToVector3(position);

			output.Add(new Cube(p, s, shader));
		}
	}
}