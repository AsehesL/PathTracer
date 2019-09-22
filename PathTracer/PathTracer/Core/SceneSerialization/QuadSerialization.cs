using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathTracer.Core.Utils;

namespace ASL.PathTracer.SceneSerialization
{
	[GeometryAnalyse(type = "Quad")]
	class QuadSerialization : GeometrySerialization
	{
		public override void GenerateGeometry(Shader shader, string scenePath, List<Geometry> output, Dictionary<string, GeometryParamData> geoParams)
		{
			string position = geoParams["Position"].paramValue;
			string normal = geoParams["Normal"].paramValue;
			string right = geoParams["Right"].paramValue;
			string up = geoParams["Up"].paramValue;

			Vector3 p = StringUtils.StringToVector3(position);
			Vector3 n = StringUtils.StringToVector3(normal);
			Vector3 r = StringUtils.StringToVector3(right);
			Vector3 u = StringUtils.StringToVector3(up);

			output.Add(new Quad(p, n, r, u, shader));
		}
	}
}
