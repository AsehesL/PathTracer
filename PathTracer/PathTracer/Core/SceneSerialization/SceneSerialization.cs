using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PathTracer.Core.Utils;

namespace ASL.PathTracer.SceneSerialization
{
    [XmlRoot("SceneRoot")]
    public class SceneDataRoot
    {
        [XmlArray("Textures")]
        [XmlArrayItem("Texture")]
        public List<TextureData> textures;

        [XmlArray("Shaders")]
        [XmlArrayItem("Shader")]
        public List<ShaderData> shaders;

        [XmlElement("Sky")] public SkyData sky;

        [XmlElement("Camera")] public CameraData camera;

        [XmlArray("Geometries")]
        [XmlArrayItem("Geometry")]
        public List<GeometryData> geometries;
    }

    public class CameraData
    {
        [XmlAttribute("Position")] public string position;

        [XmlAttribute("Euler")] public string euler;

        [XmlAttribute("Near")] public double near;

        [XmlAttribute("FieldOfView")] public double fieldOfView;

        public Camera CreateCamera()
        {
            Vector3 pos = StringUtils.StringToVector3(position);
            Vector3 rot = StringUtils.StringToVector3(euler);
            Camera cam = new Camera(pos, rot, near, fieldOfView);

            Log.Info($"相机创建成功:Position:{pos},Rotation:{rot}");
            return cam;
        }
    }

	public class TestData : GeometryData
	{
		[XmlAttribute("AOS")] public string aos;
	}

    public class GeometryData
    {
        [XmlAttribute("Shader")] public string shader;
        [XmlAttribute("Type")] public string type;

	    [XmlArray("Params")]
	    [XmlArrayItem("Param")]
	    public List<GeometryParamData> geoParams;

		public void CreateGeometry(string scenePath, List<Geometry> output, Dictionary<string, Shader> shaders)
		{
			Dictionary<string, GeometryParamData> geoParamDic = new Dictionary<string, GeometryParamData>();
			for (int i = 0; i < geoParams.Count; i++)
			{
				geoParamDic[geoParams[i].paramName] = geoParams[i];
			}

            Shader s = null;
            if (shaders.ContainsKey(shader))
                s = shaders[shader];

	        System.Type[] types = typeof(Geometry).Assembly.GetTypes();
	        System.Type geoSerializationType = typeof(GeometrySerialization);
	        GeometrySerialization geoSerialization = null;
			for (int i = 0; i < types.Length; i++)
	        {
		        if (types[i].IsAbstract)
			        continue;
		        if (types[i].IsSubclassOf(geoSerializationType))
		        {
			        var attributes = types[i].GetCustomAttributes(typeof(GeometryAnalyseAttribute), false);
			        if (attributes == null || attributes.Length == 0)
				        continue;
			        var geoattribute = attributes[0] as GeometryAnalyseAttribute;
			        if (geoattribute == null)
				        continue;
			        if (geoattribute.type == type)
			        {
				        geoSerialization = System.Activator.CreateInstance(types[i]) as GeometrySerialization;
						break;
			        }
		        }
	        }

	        if (geoSerialization == null)
	        {
		        Log.Warn($"几何体创建失败！不确定的几何体类型：{type}");
				return;
	        }

			geoSerialization.GenerateGeometry(s, scenePath, output, geoParamDic);
			//      var geo = geoSerialization.GenerateGeometry(s, geoParams);
			//      if (geo != null)
			//       output.Add(geo);
			//else
			//       Log.Warn($"几何体创建失败！");
		}
    }

	public class GeometryParamData
	{

		[XmlAttribute("Key")] public string paramName;

		[XmlAttribute("Value")] public string paramValue;

	}

	public class TextureData
    {
        [XmlAttribute("Name")]
        public string name;

        [XmlAttribute("Path")]
        public string path;

        [XmlAttribute("WrapMode")]
        public string wrapMode;

        [XmlAttribute("FilterMode")]
        public string filterMode;

        public Texture CreateTexture(string scenePath)
        {
            var fileInfo = new System.IO.FileInfo(scenePath);
            string p = System.IO.Path.Combine(fileInfo.DirectoryName, path);
            Texture tex = Texture.Create(p);
            FilterMode filterMode = FilterMode.Point;
            WrapMode wrapMode = WrapMode.Clamp;
            if (tex != null)
            {
                if (this.wrapMode == "Clamp")
                    wrapMode = WrapMode.Clamp;
                else if (this.wrapMode == "Repeat")
                    wrapMode = WrapMode.Repeat;
                if (this.filterMode == "Point")
                    filterMode = FilterMode.Point;
                else if (this.filterMode == "Bilinear")
                    filterMode = FilterMode.Bilinear;
                tex.filterMode = filterMode;
                tex.wrapMode = wrapMode;
            }
            return tex;
        }
    }

    public class ShaderData
    {
        [XmlAttribute("Name")]
        public string name;

        [XmlAttribute("ClassName")]
        public string className;

        [XmlArray("Params")]
        [XmlArrayItem("Param")]
        public List<ShaderParamData> shaderParams;

        public Shader CreateShader(Dictionary<string, Texture> textures)
        {
            var assembly = typeof(Shader).Assembly;
            var tp = assembly.GetType(className);
            if (tp == null)
            {
                Log.Warn($"Shader初时化失败！未找到该类型的Shader:{className}");
                return null;
            }

            Shader shader = System.Activator.CreateInstance(tp) as Shader;

            if (shader == null)
                return null;
            if (shaderParams != null)
            {
                for (int i = 0; i < shaderParams.Count; i++)
                {
                    var paramObj = shaderParams[i].CreateParam(textures);
                    if (paramObj == null)
                        continue;
                    shader.SetParam(shaderParams[i].paramType, shaderParams[i].paramName, paramObj);
                }
            }

            return shader;
        }
    }

    public class ShaderParamData
    {

        [XmlAttribute("Key")]
        public string paramName;

        [XmlAttribute("Value")]
        public string paramValue;

        [XmlAttribute("Type")]
        public ShaderParamType paramType;

        public System.Object CreateParam(Dictionary<string, Texture> textures)
        {
            switch (paramType)
            {
                case ShaderParamType.Color:
                    string[] colSplit = paramValue.Split(',');
                    float r = colSplit.Length > 0 ? float.Parse(colSplit[0]) : 1.0f;
                    float g = colSplit.Length > 1 ? float.Parse(colSplit[1]) : 1.0f;
                    float b = colSplit.Length > 2 ? float.Parse(colSplit[2]) : 1.0f;
                    float a = colSplit.Length > 3 ? float.Parse(colSplit[3]) : 1.0f;
                    return new Color(r, g, b, a);
                case ShaderParamType.Number:
                    float floatval = float.Parse(paramValue);
                    return floatval;
                case ShaderParamType.Texture:
                    if (textures.ContainsKey(paramValue))
                        return textures[paramValue];
                    return null;
                case ShaderParamType.Vector2:
                    string[] vec2Split = paramValue.Split(',');
                    double vec2x = vec2Split.Length > 0 ? double.Parse(vec2Split[0]) : 0.0;
                    double vec2y = vec2Split.Length > 1 ? double.Parse(vec2Split[1]) : 0.0;
                    return new Vector2(vec2x, vec2y);
                case ShaderParamType.Vector3:
                    string[] vec3Split = paramValue.Split(',');
                    double vec3x = vec3Split.Length > 0 ? double.Parse(vec3Split[0]) : 0.0;
                    double vec3y = vec3Split.Length > 1 ? double.Parse(vec3Split[1]) : 0.0;
                    double vec3z = vec3Split.Length > 2 ? double.Parse(vec3Split[2]) : 0.0;
                    return new Vector3(vec3x, vec3y, vec3z);
            }
            return null;
        }
    }

    public class SkyData
    {

        [XmlAttribute("ClassName")]
        public string className;

        [XmlArray("Params")]
        [XmlArrayItem("Param")]
        public List<ShaderParamData> shaderParams;

        public Sky CreateSky(Dictionary<string, Texture> textures)
        {
            var assembly = typeof(Sky).Assembly;
            var tp = assembly.GetType(className);
            if (tp == null)
                return null;

            Sky sky = System.Activator.CreateInstance(tp) as Sky;
            if (sky == null)
                return null;
            if (shaderParams != null)
            {
                for (int i = 0; i < shaderParams.Count; i++)
                {
                    var paramObj = shaderParams[i].CreateParam(textures);
                    if (paramObj == null)
                        continue;
                    sky.SetParam(shaderParams[i].paramType, shaderParams[i].paramName, paramObj);
                }
            }
            Log.Info($"天空盒创建成功:{tp}");
            return sky;
        }
    }

    static class SceneSerialization
    {
        public static SceneDataRoot Deserialize(string path)
        {
            if (System.IO.File.Exists(path) == false)
            {
                Log.Err($"无法定位该文件位置：" + path);
                return null;
            }

            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            XmlSerializer serializer = new XmlSerializer(typeof(SceneDataRoot));
            SceneDataRoot root = null;
            try
            {
                root = (SceneDataRoot) serializer.Deserialize(stream);
            }
            catch (System.Exception e)
            {
                Log.Err(e.Message);
            }

            stream.Dispose();
            stream.Close();

            return root;
        }
    }
}
