using ASL.PathTracer.SceneSerialization;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        protected Tracer m_Tracer;

        protected CameraBase m_Camera;

        private SceneData m_SceneData;

        private Scene(CameraBase camera, SceneData sceneData)
        {
            m_Tracer = new PathTracer(0.000001);

            m_Camera = camera;
            m_SceneData = sceneData;

            m_Tracer.sceneData = sceneData;
        }

        public static Scene Create(string scenePath)
        {
            var sceneData = ASL.PathTracer.SceneSerialization.SceneSerialization.Deserialize(scenePath);
            if (sceneData == null)
            {
                Log.Err("场景序列化失败！");
                return null;
            }

            //Scene scene = new Scene();
            var camera = sceneData.camera.CreateCamera();

            Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
            if (sceneData.textures != null)
            {
                for (int i = 0; i < sceneData.textures.Count; i++)
                {
                    var t = sceneData.textures[i].CreateTexture(scenePath);
                    if (t != null)
                        textures.Add(sceneData.textures[i].name, t);
                }
            }

            Log.Info($"纹理加载完毕：共{textures.Count}张纹理");

            var sky = sceneData.sky != null ? sceneData.sky.CreateSky(textures) : null;
            //var sun = sceneData.sun != null ? sceneData.sun.CreateSunLight() : null;
            
            Dictionary<string ,Shader> shaders = new Dictionary<string, Shader>();
            if (sceneData.shaders != null)
            {
                for (int i = 0; i < sceneData.shaders.Count; i++)
                {
                    var s = sceneData.shaders[i].CreateShader(textures);
                    if (shaders != null)
                        shaders.Add(sceneData.shaders[i].name, s);
                }
            }

            Log.Info($"Shader加载完毕：共{shaders.Count}个Shader");

            List<Geometry> geometries = new List<Geometry>();

            GeometryStats stats = new GeometryStats { totalGeometries = 0, totalTriangles = 0, };
            if (sceneData.geometries != null)
            {
                for (int i = 0; i < sceneData.geometries.Count; i++)
                {
                    sceneData.geometries[i].CreateGeometry(scenePath, geometries, shaders, ref stats);
                }
            }

            Log.Info($"几何体加载完毕：共{stats.totalGeometries}个几何体，{stats.totalTriangles}个三角形");

            var sdata = new BVH();

            Log.Info($"开始构建场景数据，场景数据类型：{sdata.GetType()}");
            sdata.Build(geometries);

            sdata.sky = sky;
            
            return new Scene(camera, sdata);
        }

        public Texture Render(int tracingTimes, SamplerType samplerType, int numSamples, uint width, uint height, RenderChannel renderChannel, int numSets = 83, System.Action<int, int> progressCallBackAction = null)
        {
            Texture result = new Texture(width, height);
            m_Tracer.tracingTimes = renderChannel == RenderChannel.Full ? tracingTimes : 0;
            #if DEBUG
            m_Tracer.isDebugging = false;
            #endif
            if (renderChannel == RenderChannel.Full)
                m_Camera.SetSampler(samplerType, numSamples, numSets);
            m_Camera.SetRenderTarget(result);
            try
            {
                m_Camera.Render(this, renderChannel, progressCallBackAction);
            }
            catch (System.Exception e)
            {
                Log.Err(e.Message);
            }

            return result;
        }


#if DEBUG
        public Texture RenderDebugSinglePixel(int x, int y, int tracingTimes, SamplerType samplerType, int numSamples, uint width, uint height, int numSets = 83)
        {
            Texture result = new Texture(width, height);
            result.Fill(Color.black);
            m_Tracer.tracingTimes = tracingTimes;
            m_Tracer.isDebugging = true;
            //m_Camera.SetSampler(samplerType, numSamples, numSets);
            var sampler = SamplerFactory.Create(samplerType, numSamples, numSets);
            m_Camera.SetRenderTarget(result);
            try
            {
                Color color = m_Camera.RenderPixelToColor(x, y, sampler, new RenderState(), RenderChannel.Full, this);
                result.SetPixel(x, y, color);
            }
            catch (System.Exception e)
            {
                Log.Err(e.Message);
            }

            return result;
        }
#endif
    }
}
