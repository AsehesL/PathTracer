using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ASL.PathTracer.SceneSerialization
{
    [XmlRoot("SceneRoot")]
    public class SceneSerializableData
    {
        [XmlArray("Textures")]
        [XmlArrayItem("Texture")]
        public List<TextureSerializableData> textures;

        [XmlArray("Materials")]
        [XmlArrayItem("Material")]
        public List<MaterialSerializableData> materials;

        [XmlElement("Sky")] public SkySerializableData sky;

        [XmlElement("SunLight")] public SunLightSerializableData sun;

        [XmlElement("Camera")] public CameraSerializableData camera;

        [XmlArray("Geometries")]
        [XmlArrayItem("Geometry")]
        public List<GeometrySerializableData> geometries;
    }

    public class CameraSerializableData
    {
        [XmlAttribute("Position")] public string position;

        [XmlAttribute("Euler")] public string euler;

        [XmlAttribute("Near")] public double near;

        [XmlAttribute("FieldOfView")] public double fieldOfView;

        [XmlAttribute("Radius")] public float radius;

        [XmlAttribute("Focal")] public float focal;

        [XmlAttribute("UseThinLens")] public bool useThinLens;

        [XmlAttribute("Width")] public int width;

        [XmlAttribute("Height")] public int height;

        public CameraBase CreateCamera(out int defaultWidth, out int defaultHeight)
        {
            defaultWidth = width;
            defaultHeight = height;

            Vector3 pos = Vector3.Parse(position);
            Vector3 rot = Vector3.Parse(euler);

            PerspectiveCamera cam = new PerspectiveCamera(pos, rot, near, fieldOfView);
            cam.useThinLens = useThinLens;
            cam.focal = focal;
            cam.radius = radius;

            if (useThinLens)
            {
                Log.Info($"相机创建成功:Position:{pos},Rotation:{rot},焦距:{focal},光圈半径:{radius}");
            }
            else
            {
                Log.Info($"相机创建成功:Position:{pos},Rotation:{rot}");
            }
            return cam;
        }
    }

    public struct MaterialName
    {
        [XmlAttribute("Name")]
        public string materialName;
    }

    public class GeometrySerializableData
    {
        [XmlAttribute("Type")] public string type;

	    [XmlArray("Params")]
	    [XmlArrayItem("Param")]
	    public List<GeometryParameterSerializableData> geoParams;

        [XmlArray("Materials")]
        [XmlArrayItem("Material")]
        public List<MaterialName> materials;

        public void CreateGeometry(string scenePath, Scene scene, List<Geometry> output)
		{
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

            List<Material> mats = new List<Material>();
            for (int i = 0; i < materials.Count; i++)
            {
                var mat = scene.GetMaterial(materials[i].materialName);
                if (mat != null)
                    mats.Add(mat);
            }

            if (geoParams != null)
            {
                for (int i = 0; i < geoParams.Count; i++)
                {
                    geoSerialization.SetParameter(geoParams[i].paramName, geoParams[i].paramValue);
                }
            }

			geoSerialization.GenerateGeometry(scenePath, scene, mats, output);
            //      var geo = geoSerialization.GenerateGeometry(s, geoParams);
            //      if (geo != null)
            //       output.Add(geo);
            //else
            //       Log.Warn($"几何体创建失败！");
		}
    }

	public class GeometryParameterSerializableData
    {

		[XmlAttribute("Key")] public string paramName;

		[XmlAttribute("Value")] public string paramValue;

	}

	public class TextureSerializableData
    {
        [XmlAttribute("Name")]
        public string name;

        [XmlAttribute("Path")]
        public string path;

        [XmlAttribute("WrapMode")]
        public string wrapMode;

        [XmlAttribute("FilterMode")]
        public string filterMode;

        [XmlAttribute("Linear")]
        public bool linear;

        public Texture CreateTexture(string scenePath)
        {
            var fileInfo = new System.IO.FileInfo(scenePath);
            string p = System.IO.Path.Combine(fileInfo.DirectoryName, path);
            Texture tex = Texture.Create(p, linear ? 1.0f : 2.2f);
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

    public class MaterialSerializableData
    {
        [XmlAttribute("Name")]
        public string name;

        [XmlAttribute("MaterialType")]
        public string materialType;

        [XmlArray("Params")]
        [XmlArrayItem("Param")]
        public List<MaterialParameterSerializableData> materialParameters;

        public Material CreateMaterial(Scene scene)
        {
            if (scene == null)
                return null;
            var assembly = typeof(Material).Assembly;
            var types = assembly.GetTypes();
            Type type = null;
            for (int i = 0; i < types.Length; i++)
            {
                if(types[i].IsAbstract || !types[i].IsSubclassOf(typeof(Material)))
                    continue;
                var attributes = types[i].GetCustomAttributes(typeof(MaterialTypeAttribute), true);
                if (attributes != null && attributes.Length > 0)
                {
                    var shaderAttribute = attributes[0] as MaterialTypeAttribute;
                    if (shaderAttribute != null && shaderAttribute.materialType == materialType)
                    {
                        type = types[i];
                        break;
                    }
                }
            }
            if (type == null)
            {
                Log.Warn($"Shader初时化失败！未找到该类型的材质:{materialType}");
                return null;
            }

            Material mat = System.Activator.CreateInstance(type) as Material;

            if (mat == null)
                return null;
            if (materialParameters != null)
            {
                for (int i = 0; i < materialParameters.Count; i++)
                {
                    var paramObj = materialParameters[i].CreateParameter(scene);
                    if (paramObj == null)
                        continue;
                    mat.SetParam(materialParameters[i].parameterType, materialParameters[i].parameterName, paramObj);
                }
            }

            return mat;
        }
    }

    public class MaterialParameterSerializableData
    {

        [XmlAttribute("Key")]
        public string parameterName;

        [XmlAttribute("Value")]
        public string parameterValue;

        [XmlAttribute("Type")]
        public MaterialParameterType parameterType;

        public System.Object CreateParameter(Scene scene)
        {
            switch (parameterType)
            {
                case MaterialParameterType.Color:
                    {
                        return Color.Parse(parameterValue);
                    }
                case MaterialParameterType.Number:
                    {
                        float floatval = float.Parse(parameterValue);
                        return floatval;
                    }
                case MaterialParameterType.Boolean:
                    {
                        bool boolval = bool.Parse(parameterValue);
                        return boolval;
                    }
                case MaterialParameterType.Texture:
                    {
                        return scene != null ? scene.GetTexture(parameterValue) : null;
                    }
                case MaterialParameterType.Vector2:
                    {
                        return Vector2.Parse(parameterValue);
                    }
                case MaterialParameterType.Vector3:
                    {
                        return Vector3.Parse(parameterValue);
                    }
                case MaterialParameterType.Vector4:
                    {
                        return Vector4.Parse(parameterValue);
                    }
            }
            return null;
        }
    }

    public class SkySerializableData
    {

        [XmlAttribute("MaterialType")]
        public string materialType;

        [XmlArray("Params")]
        [XmlArrayItem("Param")]
        public List<MaterialParameterSerializableData> materialParameters;

        public SkyLight CreateSky(Scene scene)
        {
            if (scene == null)
                return null;
            var assembly = typeof(Material).Assembly;
            var types = assembly.GetTypes();
            Type type = null;
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].IsAbstract || !types[i].IsSubclassOf(typeof(Material)))
                    continue;
                var attributes = types[i].GetCustomAttributes(typeof(MaterialTypeAttribute), true);
                if (attributes != null && attributes.Length > 0)
                {
                    var shaderAttribute = attributes[0] as MaterialTypeAttribute;
                    if (shaderAttribute != null && shaderAttribute.materialType == materialType)
                    {
                        type = types[i];
                        break;
                    }
                }
            }

            Material skyMat = System.Activator.CreateInstance(type) as Material;
            if (skyMat == null)
                return null;
            if (materialParameters != null)
            {
                for (int i = 0; i < materialParameters.Count; i++)
                {
                    var paramObj = materialParameters[i].CreateParameter(scene);
                    if (paramObj == null)
                        continue;
                    skyMat.SetParam(materialParameters[i].parameterType, materialParameters[i].parameterName, paramObj);
                }
            }

            Log.Info($"天空盒创建成功:{type}");
            
            
            return new SkyLight(skyMat);
        }
    }

    public class SunLightSerializableData
    {
        [XmlAttribute("Direction")] public string direction;

        [XmlAttribute("Color")] public string color;

        [XmlAttribute("Intensity")] public float intensity;

        [XmlAttribute("VolumetricLighting")] public bool renderParticipatingMedia;

        public SunLight CreateSunLight()
        {
            Vector3 dir = Vector3.Parse(direction);
            Color col = Color.Parse(color);

            SunLight light = new SunLight { sunDirection = dir, sunColor = col, sunIntensity = intensity, renderParticipatingMedia = renderParticipatingMedia };

            Log.Info($"创建SunLight成功:方向:{dir},颜色:{col},强度:{intensity},是否渲染参与介质:{renderParticipatingMedia}");
            return light;
        }
    }

    static class SceneSerialization
    {
        public static SceneSerializableData Deserialize(string path)
        {
            if (System.IO.File.Exists(path) == false)
            {
                Log.Err($"无法定位该文件位置：" + path);
                return null;
            }

            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            XmlSerializer serializer = new XmlSerializer(typeof(SceneSerializableData));
            SceneSerializableData root = null;
            try
            {
                root = (SceneSerializableData) serializer.Deserialize(stream);
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
