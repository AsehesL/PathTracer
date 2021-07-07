using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public static class MathEx
    {
        public const float FloatPI = 3.1415926535897931f;
        public const float FloatInvPI = 1.0f / 3.1415926535897931f;

        public const double PI = Math.PI;
        public const double InvPI = 1.0 / Math.PI;

        public static float FMin(float a, float b)
        {
            return a > b ? b : a;
        }
    }
}
