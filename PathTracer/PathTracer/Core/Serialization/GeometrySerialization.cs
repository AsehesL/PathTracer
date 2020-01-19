using System;
using System.Collections.Generic;

namespace ASL.PathTracer.SceneSerialization
{
	[System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	class GeometryAnalyseAttribute : System.Attribute
	{
		public string type;
	}

	abstract class GeometrySerialization
	{
		public abstract void GenerateGeometry(List<Shader> shaders, string scenePath, List<Geometry> output, Dictionary<string, GeometryParamData> geoParams);
	}
}
