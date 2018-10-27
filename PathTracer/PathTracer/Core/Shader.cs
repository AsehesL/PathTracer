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
        Color,
        Texture,
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
            }
        }

        public void SetFloat(string key, float value)
        {
            var field = GetType().GetField(key);
            if (field != null && field.FieldType == typeof(float))
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
    }

    public abstract class Shader : ShaderBase
    {
        public abstract Color Render(Tracer tracer, Sky sky, SamplerBase sampler, Ray ray, RayCastHit hit, double epsilon);

        public abstract Color FastRender(Ray ray, RayCastHit hit);
    }
}
