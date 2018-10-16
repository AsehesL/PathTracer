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
        Vector,
        Color,
        Texture,
    }

    public class ShaderBase
    {
        public void SetParam(ShaderParamType type, string key, string value)
        {
            switch (type)
            {
                case ShaderParamType.Color:
                    string[] colSplit = value.Split(',');
                    float r = colSplit.Length > 0 ? float.Parse(colSplit[0]) : 1.0f;
                    float g = colSplit.Length > 1 ? float.Parse(colSplit[1]) : 1.0f;
                    float b = colSplit.Length > 2 ? float.Parse(colSplit[2]) : 1.0f;
                    float a = colSplit.Length > 3 ? float.Parse(colSplit[3]) : 1.0f;
                    this.SetColor(key, new Color(r, g, b, a));
                    break;
                case ShaderParamType.Number:
                    float floatval = float.Parse(value);
                    this.SetFloat(key, floatval);
                    break;
                case ShaderParamType.Texture:
                    Texture tex = Texture.Create(value);
                    this.SetTexture(key, tex);
                    break;
                case ShaderParamType.Vector:
                    string[] vecSplit = value.Split(',');
                    double x = vecSplit.Length > 0 ? double.Parse(vecSplit[0]) : 0.0;
                    double y = vecSplit.Length > 1 ? double.Parse(vecSplit[1]) : 0.0;
                    double z = vecSplit.Length > 2 ? double.Parse(vecSplit[2]) : 0.0;
                    this.SetVector(key, new Vector3(x, y, z));
                    break;
            }
        }

        public void SetFloat(string key, float value)
        {
            var field = GetType().GetField(key);
            if (field != null && field.FieldType == typeof(float))
                field.SetValue(this, value);
        }

        public void SetVector(string key, Vector3 value)
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
        
    }
}
