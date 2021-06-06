using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public enum MaterialParameterType
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
    public class MaterialTypeAttribute : System.Attribute
    {
        public string materialType;

        public MaterialTypeAttribute(string materialType)
        {
            this.materialType = materialType;
        }
    }

    public class Material
    {
        public void SetParam(MaterialParameterType type, string key, System.Object value)
        {
            switch (type)
            {
                case MaterialParameterType.Color:
                    this.SetColor(key, (Color)value);
                    break;
                case MaterialParameterType.Number:
                    this.SetFloat(key, (float)value);
                    break;
                case MaterialParameterType.Texture:
                    this.SetTexture(key, (Texture)value);
                    break;
                case MaterialParameterType.Vector2:
                    this.SetVector2(key, (Vector2)value);
                    break;
                case MaterialParameterType.Vector3:
                    this.SetVector3(key, (Vector3)value);
                    break;
                case MaterialParameterType.Vector4:
                    this.SetVector4(key, (Vector4)value);
                    break;
                case MaterialParameterType.Boolean:
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

        public virtual bool ShouldCull(Ray ray, RayCastHit hit) { return false; }

        public virtual bool ShouldRenderBackFace() { return true; }

        public virtual bool IsEmissive() { return false; }

        public virtual Color GetEmissive(RayCastHit hit) { return Color.black; }

        public virtual float GetOcclusion(RayCastHit hit) { return 1.0f; }

        public virtual Vector3 GetWorldNormal(RayCastHit hit) { return hit.normal; }

        public virtual float GetRefractive(RayCastHit hit) { return 0.0f; }

        public virtual float GetOpacity(RayCastHit hit) { return 1.0f; }

        public virtual float GetMetallic(RayCastHit hit) { return 0.0f; }

        public virtual float GetRoughness(RayCastHit hit) { return 1.0f; }

        public virtual Color GetAlbedo(RayCastHit hit) { return Color.white; }

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
    }
}
