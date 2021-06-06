using System;
using System.Collections.Generic;

namespace ASL.PathTracer.SceneSerialization
{
	/// <summary>
	/// 定义几何体的序列化Attribute
	/// </summary>
	[System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	class GeometryAnalyseAttribute : System.Attribute
	{
		public string type;
	}

	abstract class GeometrySerialization
	{
		public bool SetParameter(string name, string value)
		{
            var field = GetType().GetField(name);
            if (field != null && field.FieldType == typeof(float))
            {
                SetFloat(field, value);
                return true;
            }
            else if (field != null && field.FieldType == typeof(bool))
            {
                SetBoolean(field, value);
                return true;
            }
            else if (field != null && field.FieldType == typeof(Vector2))
            {
                SetVector2(field, value);
                return true;
            }
            else if (field != null && field.FieldType == typeof(Vector3))
            {
                SetVector3(field, value);
                return true;
            }
            else if (field != null && field.FieldType == typeof(Vector4))
            {
                SetVector4(field, value);
                return true;
            }
            else if(field != null && field.FieldType == typeof(string))
            {
                SetString(field, value);
                return true;
            }
            return false;
        }

        private void SetFloat(System.Reflection.FieldInfo field, string value)
        {
            if (field == null)
                return;
            try
            {
                float floatvalue = float.Parse(value);
                field.SetValue(this, floatvalue);
            }
            catch(System.Exception e)
            { }
        }

        private void SetBoolean(System.Reflection.FieldInfo field, string value)
        {
            if (field == null)
                return;
            try
            {
                bool boolvalue = bool.Parse(value);
                field.SetValue(this, boolvalue);
            }
            catch (System.Exception e)
            { }
        }

        private void SetVector2(System.Reflection.FieldInfo field, string value)
        {
            if (field == null)
                return;
            try
            {
                Vector2 vec2value = Vector2.Parse(value);
                field.SetValue(this, vec2value);
            }
            catch (System.Exception e)
            { }
        }

        private void SetVector3(System.Reflection.FieldInfo field, string value)
        {
            if (field == null)
                return;
            try
            {
                Vector3 vec3value = Vector3.Parse(value);
                field.SetValue(this, vec3value);
            }
            catch (System.Exception e)
            { }
        }

        private void SetVector4(System.Reflection.FieldInfo field, string value)
        {
            if (field == null)
                return;
            try
            {
                Vector4 vec4value = Vector4.Parse(value);
                field.SetValue(this, vec4value);
            }
            catch (System.Exception e)
            { }
        }

        private void SetString(System.Reflection.FieldInfo field, string text)
        {
            if (field == null)
                return;
            try
            {
                field.SetValue(this, text);
            }
            catch (System.Exception e)
            { }
        }

        public abstract void GenerateGeometry(string scenePath, Scene scene, List<Material> materials, List<Geometry> output);
	}
}
