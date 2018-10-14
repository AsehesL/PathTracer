using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public class Scene
    {
        public Tracer tracer
        {
            get { return m_Tracer; }
        }

        public Sky sky
        {
            get { return m_Sky; }
        }

        protected Tracer m_Tracer;

        protected Sky m_Sky;

        protected Camera m_Camera;

        private SceneData m_SceneData;

        private Scene()
        {
        }

        public static Scene Create(string scenePath)
        {
            var sceneData = ASL.PathTracer.SceneSerialization.SceneSerialization.Deserialize(scenePath);
            if (sceneData == null)
                return null;

            Scene scene = new Scene();
            scene.m_Camera = sceneData.camera.CreateCamera();
            scene.m_Sky = sceneData.sky != null ? sceneData.sky.CreateSky() : null;
            
            Dictionary<string ,Shader> shaders = new Dictionary<string, Shader>();
            if (sceneData.shaders != null)
            {
                for (int i = 0; i < sceneData.shaders.Count; i++)
                {
                    shaders.Add(sceneData.shaders[i].name, sceneData.shaders[i].CreateShader());
                }
            }

            List<Geometry> geometries = new List<Geometry>();

            if (sceneData.geometries != null)
            {
                for (int i = 0; i < sceneData.geometries.Count; i++)
                {
                    sceneData.geometries[i].CreateGeometry(scenePath, geometries, shaders);
                }
            }

            scene.m_SceneData = new ListSceneData();
            scene.m_SceneData.Build(geometries);
            
            return scene;
        }

        public Texture Render(int tracingTimes, bool multiThread, SamplerType samplerType, int numSamples, int width, int height, int numSets = 83, System.Action<int, int> progressCallBackAction = null)
        {
            Texture result = new Texture(width, height);
            m_Tracer = new PathTracer(tracingTimes, 0.000001);
            m_Tracer.sceneData = m_SceneData;
            m_Camera.SetSampler(samplerType, numSamples, numSets);
            m_Camera.SetRenderTarget(result);
            m_Camera.Render(this, multiThread, progressCallBackAction);
            //m_Camera.Render(this, );

            return result;
        }
    }
}
