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
        public static Vector3 StringToVector3(string value)
        {
            string[] split = value.Split(',');
            if (split.Length != 3)
                return default(Vector3);
            double x = double.Parse(split[0]);
            double y = double.Parse(split[1]);
            double z = double.Parse(split[2]);

            return new Vector3(x, y, z);
        }
    }
}
