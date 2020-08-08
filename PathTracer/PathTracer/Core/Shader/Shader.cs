using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public enum ShaderParamType
    {
        Number,
        Vector2,
        Vector3,
        Vector4,
        Color,
        Texture,
        Boolean,
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ShaderTypeAttribute : System.Attribute
    {
        public string shaderType;

        public ShaderTypeAttribute(string shaderType)
        {
            this.shaderType = shaderType;
        }
    }

    public class ShaderBase
    {
        public void SetParam(ShaderParamType type, string key, System.Object value)
        {
            switch (type)
            {
                case ShaderParamType.Color:
                    this.SetColor(key, (Color)value);
                    break;
                case ShaderParamType.Number:
                    this.SetFloat(key, (float)value);
                    break;
                case ShaderParamType.Texture:
                    this.SetTexture(key, (Texture)value);
                    break;
                case ShaderParamType.Vector2:
                    this.SetVector2(key, (Vector2)value);
                    break;
                case ShaderParamType.Vector3:
                    this.SetVector3(key, (Vector3)value);
                    break;
                case ShaderParamType.Vector4:
                    this.SetVector4(key, (Vector4)value);
                    break;
                case ShaderParamType.Boolean:
                    this.SetBoolean(key, (bool)value);
                    break;
            }
        }

        public void SetFloat(string key, float value)
        {
            var field = GetType().GetField(key);
            if (field != null && field.FieldType == typeof(float))
                field.SetValue(this, value);
        }

        public void SetBoolean(string key, bool value)
        {
            var field = GetType().GetField(key);
            if (field != null && field.FieldType == typeof(bool))
                field.SetValue(this, value);
        }

        public void SetVector2(string key, Vector2 value)
        {
            var field = GetType().GetField(key);
            if (field != null && field.FieldType == typeof(Vector2))
                field.SetValue(this, value);
        }

        public void SetVector3(string key, Vector3 value)
        {
            var field = GetType().GetField(key);
            if (field != null && field.FieldType == typeof(Vector3))
                field.SetValue(this, value);
        }

        public void SetVector4(string key, Vector4 value)
        {
            var field = GetType().GetField(key);
            if (field != null && field.FieldType == typeof(Vector4))
                field.SetValue(this, value);
        }

        public void SetColor(string key, Color value)
        {
            var field = GetType().GetField(key);
            if (field != null && field.FieldType == typeof(Color))
                field.SetValue(this, value);
        }

        public void SetTexture(string key, Texture value)
        {
            var field = GetType().GetField(key);
            if (field != null && field.FieldType == typeof(Texture))
                field.SetValue(this, value);
        }

        protected static Vector3 ImportanceSampleGGX(Vector2 sample, float roughness)
        {
            float a = roughness * roughness;

            double phi = 2.0f * Math.PI * sample.x;
            double cos_theta = Math.Sqrt((1.0 - sample.y) / (1.0 + (a * a - 1.0) * sample.y));
            double sin_theta = Math.Sqrt(1.0 - cos_theta * cos_theta);

            return new Vector3(Math.Cos(phi) * sin_theta, Math.Sin(phi) * sin_theta, cos_theta);
        }

        protected static Vector3 ImportanceGGXDirection(Vector3 direction, SamplerBase sampler, float roughness)
        {
            Vector3 w = direction;
            Vector3 u = Vector3.Cross(new Vector3(0.00424f, 1, 0.00764f), w);
            u.Normalize();
            Vector3 v = Vector3.Cross(u, w);
            Vector3 sp = ImportanceSampleGGX(sampler.Sample(), roughness);
            Vector3 l = sp.x * u + sp.y * v + sp.z * w;
            if (Vector3.Dot(l, direction) < 0.0)
                l = -sp.x * u - sp.y * v - sp.z * w;
            l.Normalize();
            return l;
        }
    }

    public abstract class Shader : ShaderBase
    {
        public abstract Color Render(Tracer tracer, SamplerBase sampler, RenderState renderState, Ray ray, RayCastHit hit);

        public abstract Color RenderEmissiveOnly(Tracer tracer, SamplerBase sampler, RenderState renderState, Ray ray, RayCastHit hit);

        public abstract Color RenderPreviewChannel(Tracer tracer, Ray ray, RayCastHit hit, RenderChannel renderChannel);

        public virtual bool ShouldCull(Ray ray, RayCastHit hit, double epsilon) { return false; }

        protected static Vector3 RecalucateNormal(Vector3 normal, Vector4 tangent, Color normalColor)
        {
            Vector3 wbnormal = Vector3.Cross(normal, tangent.xyz).normalized * tangent.w;

            Vector3 rnormal = new Vector3(normalColor.r * 2 - 1, normalColor.g * 2 - 1, normalColor.b * 2 - 1) * -1;
            Vector3 worldnormal = default(Vector3);
            worldnormal.x = tangent.x * rnormal.x + wbnormal.x * rnormal.y + normal.x * rnormal.z;
            worldnormal.y = tangent.y * rnormal.x + wbnormal.y * rnormal.y + normal.y * rnormal.z;
            worldnormal.z = tangent.z * rnormal.x + wbnormal.z * rnormal.y + normal.z * rnormal.z;
            worldnormal.Normalize();
            if (Vector3.Dot(worldnormal, normal) < 0)
                worldnormal *= -1;
            return worldnormal;
        }

        protected static float FresnelSchlickRoughness(float cosTheta, float F0, float roughness)
        {
            return F0 + (Math.Max(1.0f - roughness, F0) - F0) * (float)Math.Pow(1.0f - cosTheta, 5.0f);
        }

        protected static Vector3 FresnelSchlickRoughness(float cosTheta, Vector3 F0, float roughness)
        {
            Vector3 f = new Vector3(Math.Max(1.0f - roughness, F0.x), Math.Max(1.0f - roughness, F0.y),
                Math.Max(1.0f - roughness, F0.z));
            return F0 + (f - F0) * (float)Math.Pow(1.0f - cosTheta, 5.0f);
        }

        protected static float FresnelSchlickRoughnessRefractive(float cosTheta, float refractive, float roughness)
        {
            float F0 = (1.0f - refractive) / (1.0f + refractive);
            F0 = F0 * F0;
            return F0 + (Math.Max(1.0f - roughness, F0) - F0) * (float)Math.Pow(1.0f - cosTheta, 5.0f);
        }
    }
}
