using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    class DiffuseShader : Shader
    {
        public Color color;
        public float e;

        private float m_InvPi;

        public DiffuseShader() : base()
        {
            m_InvPi = (float) (1.0f/Math.PI);
        }

        //public Color SampleF(SamplerBase sampler, Ray ray, RayCastHit hit, out Vector3 refl)
        //{
        //    refl = default(Vector3);

        //}

        public override Color Render(Tracer tracer, Sky sky, SamplerBase sampler, Ray ray, RayCastHit hit, double epsilon)
        {
            throw new NotImplementedException();
        }
    }
}
