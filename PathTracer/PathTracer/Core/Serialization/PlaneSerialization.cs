using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathTracer.Core.Utils;

namespace ASL.PathTracer.SceneSerialization
{
	/// <summary>
	/// 平面序列化器
	/// </summary>
	[GeometryAnalyse(type = "Plane")]
	class PlaneSerialization : GeometrySerialization
	{
		public override void GenerateGeometry(List<Shader> shaders, string scenePath, List<Geometry> output, Dictionary<string, GeometryParamData> geoParams, ref GeometryStats stats)
		{
			string position = geoParams["Position"].paramValue;
			string normal = geoParams["Normal"].paramValue;

			Vector3 pos = StringUtils.StringToVector3(position);
			Vector3 n = StringUtils.StringToVector3(normal);

			output.Add(new Plane(pos, n, shaders[0]));

			stats.totalGeometries++;
		}
	}
}
