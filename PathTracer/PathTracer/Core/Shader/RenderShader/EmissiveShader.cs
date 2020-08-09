using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{

    [ShaderType("Emissive")]
    class EmissiveShader : Shader
    {
        public Color color;
        public float intensity;

        public override Color RenderEmissiveOnly(Tracer tracer, SamplerBase sampler, RenderState renderState, Ray ray, RayCastHit hit)
        {
            Color col = color * intensity;

            return col;
        }

        public override Color Render(Tracer tracer, SamplerBase sampler, RenderState renderState, Ray ray, RayCastHit hit)
        {
            Color col = color * intensity;

            return col;
        }

        public override Color RenderPreviewChannel(Tracer tracer, SamplerBase sampler, Ray ray, RayCastHit hit, RenderChannel renderChannel)
        {
            switch (renderChannel)
            {
                case RenderChannel.WorldNormal:
                    {
                        Vector3 worldNormal = hit.normal;
                        return new Color((float)worldNormal.x * 0.5f + 0.5f, (float)worldNormal.y * 0.5f + 0.5f, (float)worldNormal.z * 0.5f + 0.5f);
                    }
                case RenderChannel.DiffuseNoLighting:
                case RenderChannel.Diffuse:
                case RenderChannel.Emissive:
                    return color * intensity;
                case RenderChannel.Occlusion:
                case RenderChannel.Alpha:
                    return Color.white;
            }
            return Color.black;
        }
    }
}