using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    [ShaderType("Environment")]
    class AmbientSphere : Sky
    {
        public Texture environment;
        public float intensity = 1.0f;

        public AmbientSphere() : base()
        {
        }

        public override Color RenderColor(Vector3 dir)
        {
            if (environment == null)
                return Color.white;
            float fi = (float)Math.Atan2(dir.x, dir.z);
            float u = fi*0.5f*(float)MathUtils.InvPi;
            float theta = (float)Math.Acos(dir.y);

            float v = 1.0f - theta* (float)MathUtils.InvPi;

            //double ldv = Math.Max(0, Vector3.Dot(dir, new Vector3(0.2556, 0.9409, 0.2222)));
            //ldv = Math.Pow(ldv, 2.2);

            return environment.Sample(u, v) * intensity;// + new Color(1, 0.9f, 0.8f) * (float)ldv * 3.8f;
        }
    }
}