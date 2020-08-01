using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public abstract class Sky : ShaderBase
    {
        public bool hasSun;

        public Vector3 sunDirection;

        public Color sunColor;

        public float sunIntensity;

        public abstract Color RenderColor(Vector3 dir);
    }
}
