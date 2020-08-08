using System;
using System.Collections.Generic;

namespace ASL.PathTracer.SceneSerialization
{
	/// <summary>
	/// 定义几何体的序列化Attribute
	/// </summary>
	[System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	class GeometryAnalyseAttribute : System.Attribute
	{
		public string type;
	}

	public struct GeometryStats
	{
		public int totalGeometries;
		public int totalTriangles;
	}

	abstract class GeometrySerialization
	{
		public abstract void GenerateGeometry(List<Shader> shaders, string scenePath, List<Geometry> output, Dictionary<string, GeometryParamData> geoParams, ref GeometryStats stats);
	}
}
