//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ASL.PathTracer
//{
//    [ShaderType("CamTest")]
//    class CamTestShader : Shader
//    {
//        public Color color;

//        public override Color RenderEmissiveOnly(Tracer tracer, SamplerBase sampler, RenderState renderState, Ray ray, RayCastHit hit)
//        {
//            return Color.black;
//        }

//        public override Color Render(Tracer tracer, SamplerBase sampler, RenderState renderState, Ray ray, RayCastHit hit)
//        {
//            float vdn = (float)Math.Max(0, Vector3.Dot(-1.0 * ray.direction, hit.normal));
//            Color difcol = color;

//            difcol *= vdn;
//            return difcol;
//        }

//        public override Color FastRender(Ray ray, RayCastHit hit)
//        {
//            float vdn = (float)Math.Max(0, Vector3.Dot(-1.0 * ray.direction, hit.normal));
//            Color difcol = color;

//            difcol *= vdn;
//            return difcol;
//        }
//    }
//}