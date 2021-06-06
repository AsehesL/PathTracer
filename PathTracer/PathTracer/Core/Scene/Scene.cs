using ASL.PathTracer.SceneSerialization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public class Scene
    {
        public CameraBase camera;

        public SceneData sceneData;

        public SkyLight skyLight;

        public List<Light> lights;

        private Dictionary<string, Texture> m_Textures;

        private Dictionary<string, Material> m_Materials;

        private Scene()
        {
        }

        public Texture GetTexture(string name)
        {
            if (m_Textures != null && m_Textures.ContainsKey(name))
                return m_Textures[name];
            return null;
        }

        public Material GetMaterial(string name)
        {
            if (m_Materials != null && m_Materials.ContainsKey(name))
                return m_Materials[name];
            return null;
        }

        public void AddLight(Light light)
        {
            if (light == null)
                return;
            if (lights == null)
                lights = new List<Light>();
            lights.Add(light);
        }

        public static Scene Create(string scenePath)
        {
            int dwidth, dheight;
            return Create(scenePath, out dwidth, out dheight);
        }

        public static Scene Create(string scenePath, out int defaultWidth, out int defaultHeight)
        {
            defaultWidth = 0;defaultHeight = 0;

            var sceneData = ASL.PathTracer.SceneSerialization.SceneSerialization.Deserialize(scenePath);
            if (sceneData == null)
            {
                Log.Err("场景序列化失败！");
                return null;
            }

            Scene scene = new Scene();
            var camera = sceneData.camera.CreateCamera(out defaultWidth, out defaultHeight);
            scene.camera = camera;

            scene.m_Textures = new Dictionary<string, Texture>();
            if (sceneData.textures != null)
            {
                for (int i = 0; i < sceneData.textures.Count; i++)
                {
                    var t = sceneData.textures[i].CreateTexture(scenePath);
                    if (t != null)
                        scene.m_Textures.Add(sceneData.textures[i].name, t);
                }
            }

            Log.Info($"纹理加载完毕：共{scene.m_Textures.Count}张纹理");

            var sky = sceneData.sky != null ? sceneData.sky.CreateSky(scene) : null;
            var sun = sceneData.sun != null ? sceneData.sun.CreateSunLight() : null;

            scene.skyLight = sky;
            //scene.AddLight(sun);

            scene.m_Materials = new Dictionary<string, Material>();
            if (sceneData.materials != null)
            {
                for (int i = 0; i < sceneData.materials.Count; i++)
                {
                    var s = sceneData.materials[i].CreateMaterial(scene);
                    if (scene.m_Materials != null)
                        scene.m_Materials.Add(sceneData.materials[i].name, s);
                }
            }

            Log.Info($"材质加载完毕：共{scene.m_Materials.Count}个材质");

            List<Geometry> geometries = new List<Geometry>();

            if (sceneData.geometries != null)
            {
                for (int i = 0; i < sceneData.geometries.Count; i++)
                {
                    sceneData.geometries[i].CreateGeometry(scenePath, scene, geometries);
                }
            }

            Log.Info($"几何体加载完毕：共{sceneData.geometries.Count}个几何体");

            foreach (var geometry in geometries)
            {
                if (geometry != null && geometry.material != null && geometry.material.IsEmissive())
                {
                    AreaLight light = new AreaLight(geometry);
                    scene.AddLight(light);
                }
            }

            if (scene.lights != null && scene.lights.Count > 0)
                Log.Info($"灯光创建完毕：共{scene.lights.Count}盏灯光");

            var sdata = new BVHSceneData();

            Log.Info($"开始构建场景数据，场景数据类型：{sdata.GetType()}");
            sdata.Build(geometries);

            scene.sceneData = sdata;
            
            return scene;
        }
    }
}
