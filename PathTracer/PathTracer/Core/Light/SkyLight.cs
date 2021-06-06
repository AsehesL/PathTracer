using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public class SkyLight : Light
    {
        public Material material
        {
            get
            {
                return m_Material;
            }
        }

        private Material m_Material;

        public SkyLight(Material material)
        {
            m_Material = material;
        }

        public override Vector3 GetNormal(Vector3 hitPoint, Vector3 surfacePoint)
        {
            return default(Vector3);
        }

        public override float GetPDF()
        {
            return 0;//cos(theta)/PI
        }

        public override Vector3 Sample(SamplerBase sampler, Vector3 hitPoint)
        {
            return default(Vector3);
        }

        public override bool L(SceneData sceneData, Ray ray, out Color color, out Vector3 normal)
        {
            color = Color.black;
            normal = default(Vector3);
            return true;
        }
    }
}
