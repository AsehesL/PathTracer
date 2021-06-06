using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    [MaterialType("Transparency")]
    public class TransparencyMaterial : Material
    {
        public Color color = Color.white;
        public float refractive;
        public float roughness;
        public Vector2 tile;
        public Texture bump;

        public override bool ShouldRenderBackFace()
        {
            return true;
        }

        public override Color GetAlbedo(RayCastHit hit)
        {
            return color;
        }

        public override float GetRoughness(RayCastHit hit)
        {
            return roughness;
        }

        public override float GetRefractive(RayCastHit hit)
        {
            return refractive;
        }

        public override float GetOpacity(RayCastHit hit)
        {
            return 0.0f;
        }

        public override Vector3 GetWorldNormal(RayCastHit hit)
        {
            if (bump != null)
                return RecalucateNormal(hit.normal, hit.tangent, bump.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y)));
            return hit.normal;
        }
    }
}
