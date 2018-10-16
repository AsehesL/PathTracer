using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    class AmbientSphere : Sky
    {
        public Texture environment;
        public float intensity = 1.0f;

        private float m_InvPi;

        public AmbientSphere() : base()
        {
            m_InvPi = (float) (1.0f/Math.PI);
        }

        public override Color RenderColor(Vector3 dir)
        {
            if (environment == null)
                return Color.white;
            float fi = (float)Math.Atan2(dir.x, dir.z);
            float u = fi*0.5f*m_InvPi;
            float theta = (float)Math.Acos(dir.y);

            float v = 1.0f - theta*m_InvPi;

            return environment.Sample(u, v) * intensity;
        }
    }
}