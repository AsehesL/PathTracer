using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    static class Fresnel
    {
        public static float FresnelSchlick(float cosTheta, float F0, float roughness)
        {
            return F0 + (Math.Max(1.0f - roughness, F0) - F0) * (float)Math.Pow(1.0f - cosTheta, 5.0f);
        }

        public static Color FresnelSchlick(float cosTheta, Color F0, float roughness)
        {
            Color f = new Color(Math.Max(1.0f - roughness, F0.r), Math.Max(1.0f - roughness, F0.g),
                Math.Max(1.0f - roughness, F0.b));
            return F0 + (f - F0) * (float)Math.Pow(1.0f - cosTheta, 5.0f);
        }
    }
}
