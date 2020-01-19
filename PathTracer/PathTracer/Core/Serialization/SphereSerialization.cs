using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathTracer.Core.Utils;

namespace ASL.PathTracer.SceneSerialization
{
	[GeometryAnalyse(type = "Sphere")]
	class SphereSerialization : GeometrySerialization
	{
		public override void GenerateGeometry(List<Shader> shaders, string scenePath, List<Geometry> output, Dictionary<string, GeometryParamData> geoParams)
		{
			string position = geoParams["Position"].paramValue;
			string radius = geoParams["Radius"].paramValue;
			float r = float.Parse(radius);
			Vector3 p = StringUtils.StringToVector3(position);

			output.Add(new Sphere(p, r, shaders[0]));
		}
	}
}
