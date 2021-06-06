using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    [MaterialType("Environment")]
    public class EnvironmentSphereMaterial : Material
    {
        public Texture environment;
        public Color color = Color.white;
        public float intensity = 1.0f;

        public override bool IsEmissive()
        {
            return true;
        }

        public override Color GetEmissive(RayCastHit hit)
        {
            if (environment == null)
                return color * intensity;
            float fi = (float)Math.Atan2(hit.normal.x, hit.normal.z);
            float InvPi = (float)(1.0 / Math.PI);
            float u = fi * 0.5f * InvPi;
            float theta = (float)Math.Acos(hit.normal.y);

            float v = 1.0f - theta * InvPi;

            return environment.Sample(u, v) * color * intensity;
        }
    }
}
