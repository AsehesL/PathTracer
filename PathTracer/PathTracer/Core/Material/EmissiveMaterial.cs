using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    [MaterialType("Emissive")]
    public class EmissiveMaterial : Material
    {
        public Color color;
        public float intensity;
        public bool renderBackFace;

        public override bool IsEmissive()
        {
            return true;
        }

        public override Color GetEmissive(RayCastHit hit)
        {
            return color * intensity;
        }

        public override bool ShouldRenderBackFace()
        {
            return renderBackFace;
        }
    }
}
