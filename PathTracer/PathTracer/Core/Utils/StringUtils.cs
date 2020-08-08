using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASL.PathTracer;

namespace PathTracer.Core.Utils
{
    static class StringUtils
    {
        public static Vector2 StringToVector2(string value)
        {
            string[] split = value.Split(',');
            
            double x = split.Length >= 1 ? double.Parse(split[0]) : 0.0;
            double y = split.Length >= 2 ? double.Parse(split[1]) : 0.0;

            return new Vector2(x, y);
        }

        public static Vector3 StringToVector3(string value)
        {
            string[] split = value.Split(',');

            double x = split.Length >= 1 ? double.Parse(split[0]) : 0.0;
            double y = split.Length >= 2 ? double.Parse(split[1]) : 0.0;
            double z = split.Length >= 3 ? double.Parse(split[2]) : 0.0;

            return new Vector3(x, y, z);
        }

        public static Vector4 StringToVector4(string value)
        {
            string[] split = value.Split(',');

            double x = split.Length >= 1 ? double.Parse(split[0]) : 0.0;
            double y = split.Length >= 2 ? double.Parse(split[1]) : 0.0;
            double z = split.Length >= 3 ? double.Parse(split[2]) : 0.0;
            double w = split.Length >= 4 ? double.Parse(split[3]) : 0.0;

            return new Vector4(x, y, z, w);
        }

        public static Color StringToColor(string value)
        {
            string[] split = value.Split(',');

            float r = split.Length >= 1 ? float.Parse(split[0]) : 0.0f;
            float g = split.Length >= 2 ? float.Parse(split[1]) : 0.0f;
            float b = split.Length >= 3 ? float.Parse(split[2]) : 0.0f;
            float a = split.Length >= 4 ? float.Parse(split[3]) : 1.0f;

            Color color = new Color(r, g, b, a);
            color.Gamma(2.2f);
            return color;
        }
    }
}
