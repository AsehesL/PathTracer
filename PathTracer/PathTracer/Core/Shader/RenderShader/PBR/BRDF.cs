using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    abstract class BRDFBase
    {
        public abstract float BRDF(Vector3 inDir, Vector3 outDir, Vector3 normal, float roughness);

        public abstract float BRDFDirectional(Vector3 inDir, Vector3 outDir, Vector3 normal, float roughness);
    }

    class LambertatianBRDF : BRDFBase
    {
        public override float BRDF(Vector3 inDir, Vector3 outDir, Vector3 normal, float roughness)
        {
            return (float)MathUtils.InvPi;
        }

        public override float BRDFDirectional(Vector3 inDir, Vector3 OoutDirutDir, Vector3 normal, float roughness)
        {
            return (float)MathUtils.InvPi;
        }
    }

    class CookTorranceBRDF : BRDFBase
    {
        public override float BRDF(Vector3 inDir, Vector3 outDir, Vector3 normal, float roughness)
        {
            double denominator = 4.0 * Math.Max(Vector3.Dot(normal, inDir), 0.0) * Math.Max(Vector3.Dot(normal, outDir), 0.0) + 0.001;

            float nominator = G_SmithGGX(inDir, outDir, normal, roughness);

            return nominator / (float)denominator;
        }

        public override float BRDFDirectional(Vector3 inDir, Vector3 outDir, Vector3 normal, float roughness)
        {
            double denominator = 4.0 * Math.Max(Vector3.Dot(normal, inDir), 0.0) * Math.Max(Vector3.Dot(normal, outDir), 0.0) + 0.001;

            float nominator = D_GGX((inDir + outDir).normalized, normal, roughness) * G_SmithGGX(inDir, outDir, normal, roughness);

            return nominator / (float)denominator;
        }

        public float D_GGX(Vector3 half, Vector3 normal, float roughness)
        {
            float NdotH = (float)Vector3.Dot(half, normal);

            float root = roughness / (NdotH * NdotH * (roughness * roughness - 1.0f) + 1.0f);

            return (float)MathUtils.InvPi * (root * root);
        }

        public float G_SmithGGX(Vector3 inDir, Vector3 outDir, Vector3 normal, float roughness)
        {
            float NdotV = (float)Math.Max(0, Vector3.Dot(inDir, normal));
            float NdotL = (float)Math.Max(0, Vector3.Dot(outDir, normal));

            float ggx2 = GeometrySchlickGGX(NdotV, roughness);
            float ggx1 = GeometrySchlickGGX(NdotL, roughness);

            return ggx1 * ggx2;
        }

        private float GeometrySchlickGGX(float NdotV, float roughness)
        {
            float r = (roughness + 1.0f);
            float k = (r * r) / 8.0f;

            float nom = NdotV;
            float denom = NdotV * (1.0f - k) + k;

            return nom / denom;
        }
        
    }
}
