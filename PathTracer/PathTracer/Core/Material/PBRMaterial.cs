using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    [MaterialType("PBR")]
    public class PBRMaterial : Material
    {
        public Color color;
        public Texture albedo;
        public float metallic;
        public float roughness;
        public Color emissive;
        public Texture emissiveTex;
        public float refractive;
        public Vector2 tile;
        public Texture bump;
        public bool transparent;
        public Texture mroTex;
        public bool transparentCutOut;

        public override Color GetAlbedo(RayCastHit hit)
        {
            Color albedoColor = color;
            if (albedo != null)
                albedoColor *= albedo.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y));
            return albedoColor;
        }

        public override Color GetEmissive(RayCastHit hit)
        {
            Color emissiveColor = emissive;
            if (emissiveTex != null)
            {
                emissiveColor *= emissiveTex.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y));
            }
            return emissiveColor;
        }

        public override float GetMetallic(RayCastHit hit)
        {
            if (mroTex != null)
            {
                Color mro = mroTex.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y));
                return mro.r;
            }
            return metallic;
        }

        public override float GetOcclusion(RayCastHit hit)
        {
            if (mroTex != null)
            {
                Color mro = mroTex.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y));
                return mro.b;
            }
            return 1.0f;
        }

        public override float GetOpacity(RayCastHit hit)
        {
            return GetAlbedo(hit).a;
        }

        public override float GetRefractive(RayCastHit hit)
        {
            return refractive;
        }

        public override float GetRoughness(RayCastHit hit)
        {
            if (mroTex != null)
            {
                Color mro = mroTex.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y));
                return mro.g;
            }
            return roughness;
        }

        public override Vector3 GetWorldNormal(RayCastHit hit)
        {
            if (bump != null)
                return RecalucateNormal(hit.normal, hit.tangent, bump.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y)));
            return hit.normal;
        }

        public override bool ShouldCull(Ray ray, RayCastHit hit)
        {
            if (!transparentCutOut)
                return false;
            if (GetOpacity(hit) < 0.5f)
                return true;
            return false;
        }

        public override bool ShouldRenderBackFace()
        {
            return true;
        }

        public override bool IsEmissive()
        {
            return false;
        }
    }
}
