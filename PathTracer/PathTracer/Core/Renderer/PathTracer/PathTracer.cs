using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ASL.PathTracer
{

    public struct PathTracerRenderJobResult : IRenderJobResult
    {
        private Color m_Color;
        private int m_X;
        private int m_Y;

        public PathTracerRenderJobResult(int x, int y, Color color)
        {
            m_Color = color;
            m_X = x;
            m_Y = y;
        }

        //public void SetPixel(int x, int y, Color color)
        //{
        //    int i = x - m_X;
        //    int j = y - m_Y;
        //    m_Colors[j * m_TileWidth + i] = color;
        //}

        //public Color GetPixel(int i, int j)
        //{
        //    return m_Colors[j * m_TileWidth + i];
        //}

        public bool ApplyToRenderResult(IRenderResult result)
        {
            Texture texture = result as Texture;
            if (texture != null)
            {
                //for(int i=0;i< m_TileWidth; i++)
                //{
                //    for(int j=0;j< m_TileHeight; j++)
                //    {
                //        Color color = m_Colors[j * m_TileWidth + i];
                //        texture.SetPixel(m_X + i, m_Y + j, color);
                //    }
                //}
                texture.SetPixel(m_X, m_Y, m_Color);
                return true;
            }
            return false;
        }
    }

    struct PathTracerRenderWork : IRenderWork<PathTracerRenderJobResult, PathTracer>
    {
        private int m_TileX;
        private int m_TileY;

        private int m_I;
        private int m_J;

        private int m_Width;
        private int m_Height;

        public PathTracerRenderWork(int tileX, int tileY, int width, int height)
        {
            m_TileX = tileX;
            m_TileY = tileY;

            m_Width = width;
            m_Height = height;

            m_I = m_TileX;
            m_J = m_TileY;
        }

        public bool IsRenderComplete()
        {
            if (m_J < m_Height && m_J - m_TileY < 32)
            {
                if (m_I < m_Width && m_I - m_TileX < 32)
                {
                    return false;
                }
            }
            return true;
        }

        public PathTracerRenderJobResult Render(Scene scene, PathTracer renderer, SamplerBase sampler)
        {
            //int tileWidth = m_Width - m_TileX;
            //if (tileWidth > 32)
            //    tileWidth = 32;
            //int tileHeight = m_Height - m_TileY;
            //if (tileHeight > 32)
            //    tileHeight = 32;
            //PathTracerRenderJobResult result = new PathTracerRenderJobResult(m_I, m_J, tileWidth, tileHeight);

            //for (; m_J < m_Height && m_J - m_TileY < 32;m_J++)
            //{
            //    for(; m_I < m_Width && m_I - m_TileX < 32;m_I++)
            //    {
            //        Color col = Color.black;
            //        sampler.ResetSampler(); //重置采样器状态
            //        while (sampler.NextSample())
            //        {
            //            Ray ray = scene.camera.GetRay(m_I, m_J, sampler);
            //            col += renderer.PathTracing(ray, sampler);
            //        }
            //        col /= sampler.numSamples;

            //        col.a = 1.0f;

            //        result.SetPixel(m_I, m_J, col);
            //    }
            //    if (m_I >= m_Width || m_I - m_TileX >= 32)
            //    {
            //        m_I = m_TileX;
            //    }
            //}
            Color col = Color.black;
            sampler.ResetSampler(); //重置采样器状态
            while (sampler.NextSample())
            {
                Ray ray = scene.camera.GetRay(m_I, m_J, sampler);
                col += renderer.PathTracing(ray, sampler);
            }
            col /= sampler.numSamples;
            col.a = 1.0f;

            PathTracerRenderJobResult result = new PathTracerRenderJobResult(m_I, m_J, col);

            m_I++;
            if (m_I >= m_Width || m_I - m_TileX >= 32)
            {
                m_I = m_TileX;
                m_J++;
            }

            return result;
        }
    }

    /// <summary>
    /// 路径追踪器
    /// </summary>
    public class PathTracer : RendererBase
    {
        public bool sampleDirectLight;

        private PathTracerShader m_Shader;
        private PathTracerSkyShader m_SkyShader;

        public PathTracer(Scene scene, bool sampleDirectLight = true) : base(scene)
        {
            m_Shader = new PathTracerShader(this);
            m_SkyShader = new PathTracerSkyShader(this);
            this.sampleDirectLight = false;// sampleDirectLight;
        }

        protected override IRenderResult OnRender(RenderConfig config, RenderJobCallBackDelegate progressCallBackAction)
        {
            CameraBase camera = m_Scene.camera;
            if (camera == null)
                throw new System.ArgumentNullException();

            Texture result = new Texture(config.width, config.height);
            camera.SetRenderTarget(result);

            RenderJob<PathTracerRenderJobResult, PathTracer> job = new RenderJob<PathTracerRenderJobResult, PathTracer>(config, m_Scene, this);

            for (int j = 0; j < camera.renderTarget.height; j += 32)
            {
                for (int i = 0; i < camera.renderTarget.width; i += 32)
                {
                    job.AddWork(new PathTracerRenderWork(i, j, (int)config.width, (int)config.height));
                }
            }
            job.Render(camera.renderTarget, progressCallBackAction);

            return result;
        }

#if DEBUG
        protected override IRenderResult OnRenderDebugSinglePixel(int x, int y, RenderConfig config)
        {
            CameraBase camera = m_Scene.camera;
            if (camera == null)
                throw new System.ArgumentNullException();

            Texture result = new Texture(config.width, config.height);
            result.Fill(Color.black);

            var sampler = SamplerFactory.Create(config.samplerType, config.numSamples, config.numSets);
            camera.SetRenderTarget(result);


            Color col = Color.black;
            sampler.ResetSampler(); //重置采样器状态
            while (sampler.NextSample())
            {
                Ray ray = m_Scene.camera.GetRay(x, y, sampler);
                col += this.PathTracing(ray, sampler);
            }

            col /= sampler.numSamples;

            col.a = 1.0f;
            result.SetPixel(x, y, col);

            return result;
        }
#endif

        public Color PathTracing(Ray ray, SamplerBase sampler, int depth = 0)
        {
            if (scene == null)
                return Color.black;

            RayCastHit hit;
            hit.distance = double.MaxValue;

            Color color = Color.black;

            if (scene.sceneData.Raycast(ray, out hit))
            {
                hit.depth = depth;
                if (hit.material == null)
                    color = new Color(1, 0, 1);
                else
                {
                    color = m_Shader.Shade(hit.material, sampler, ray, hit);
                }
            }
            else
            {
                if (depth > tracingTimes)
                    color = Color.black;
                else if (scene.skyLight != null)
                    color = m_SkyShader.Shade(scene.skyLight.material, sampler, ray);
            }

            color.FixColor();

#if DEBUG
            if (isDebugging)
            {
                Log.AddLog(LogType.Debugging, $"深度：{depth}，射线：{ray}，颜色：{color}");
            }
#endif
            return color;
        }

        public bool ShadowTracing(Ray ray)
        {
            RayCastHit hit;
            hit.distance = double.MaxValue;

            if (scene.sceneData.Raycast(ray, out hit))
            {
                return true;
            }
            return false;
        }
    }
}
