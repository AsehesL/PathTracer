using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public struct Color
    {
        public float r, g, b, a;

        public float grayScale
        {
            get { return 0.299f*r + 0.587f*g + 0.114f*b; }
        }

        public static Color black
        {
            get { return new Color(0, 0, 0, 1); }
        }

        public static Color white
        {
            get { return new Color(1, 1, 1, 1); }
        }

        public static Color maroon
        {
            get { return new Color(1, 0, 1, 1); }
        }

        public static Color mediumBlue
        {
            get { return new Color(0xff191970); }
        }

        public static Color darkOrange
        {
            get { return new Color(0xffff8c00); }
        }

        public static Color yellow
        {
            get { return new Color(1, 1, 0, 1); }
        }

        public static Color green
        {
           get { return new Color(0, 1, 0, 1); }
        }

        public static Color cyan
        {
            get { return new Color(0, 1, 1, 1); }
        }

        public float this[int index]
        {
            get
            {
                if (index == 0)
                    return r;
                else if (index == 1)
                    return g;
                else if (index == 2)
                    return b;
                else if (index == 3)
                    return a;
                throw new System.IndexOutOfRangeException();
            }
            set
            {
                if (index == 0)
                    r = value;
                else if (index == 1)
                    g = value;
                else if (index == 2)
                    b = value;
                else if (index == 3)
                    a = value;
                throw new System.IndexOutOfRangeException();
            }
        }

        public Color(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public Color(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = 1.0f;
        }

        public Color(uint argb)
        {
            int a32 = (int)((argb & 0xff000000) >> 24);
            int r32 = (int)((argb & 0xff0000) >> 16);
            int g32 = (int)((argb & 0xff00) >> 8);
            int b32 = (int)(argb & 0xff);

            this.r = ((float)r32) / 255.0f;
            this.g = ((float)g32) / 255.0f;
            this.b = ((float)b32) / 255.0f;
            this.a = ((float)a32) / 255.0f;
        }

        public bool IsCloseToZero()
        {
            if (r >= float.Epsilon) return false;
            if (g >= float.Epsilon) return false;
            if (b >= float.Epsilon) return false;
            return true;
        }

        public int ToARGB()
        {
            int red = (int)(r * 255.0f);
            int green = (int)(g * 255.0f);
            int blue = (int)(b * 255.0f);
            int alpha = (int)(a * 255.0f);
            if (red > 255)
                red = 255;
            if (red < 0)
                red = 0;

            if (green > 255)
                green = 255;
            if (green < 0)
                green = 0;

            if (blue > 255)
                blue = 255;
            if (blue < 0)
                blue = 0;

            if (alpha > 255)
                alpha = 255;
            if (alpha < 0)
                alpha = 0;

            return (int)((alpha << 24) | (red << 16) | (green << 8) | (blue));
        }

        public void ToHSV(ref float h, ref float s, ref float v)
        {
            Color.RGBToHSV(this, ref h, ref s, ref v);
        }

        public float Luminance()
        {
            return r * 0.22f + g * 0.707f + b * 0.071f;
        }

        public float Average()
        {
            return (r + g + b) / 3.0f;
        }

        public void ClampColor()
        {
            this.FixColor();
            if (r > 1.0f) r = 1.0f;
            if (g > 1.0f) g = 1.0f;
            if (b > 1.0f) b = 1.0f;
            if (a > 1.0f) a = 1.0f;
        }

        public void FixColor()
        {
            if (float.IsNaN(r) || r < 0.0f)
                r = 0.0f;
            if (float.IsNaN(g) || g < 0.0f)
                g = 0.0f;
            if (float.IsNaN(b) || b < 0.0f)
                b = 0.0f;
            if (float.IsNaN(a) || a < 0.0f)
                a = 0.0f;
        }

        public void Tonemapping(float exposure)
        {
            this.r = TonemappingChannel(this.r, exposure);
            this.g = TonemappingChannel(this.g, exposure);
            this.b = TonemappingChannel(this.b, exposure);
            this.a = TonemappingChannel(this.a, exposure);
        }

        private float TonemappingChannel(float channel, float exposure)
        {
            channel *= exposure;
            return (channel * (2.51f * channel + 0.03f)) / (channel * (2.43f * channel + 0.59f) + 0.14f);
        }

        public void Gamma(float gamma)
        {
            this.r = (float)Math.Pow(r, gamma);
            this.g = (float)Math.Pow(g, gamma);
            this.b = (float)Math.Pow(b, gamma);
            this.a = (float)Math.Pow(a, gamma);
        }

        public static Color operator +(Color a, Color b)
        {
            return new Color(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a);
        }

        public static Color operator -(Color a, Color b)
        {
            return new Color(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a);
        }

        public static Color operator *(Color a, Color b)
        {
            return new Color(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
        }

        public static Color operator *(Color color, float scale)
        {
            return new Color(color.r * scale, color.g * scale, color.b * scale, color.a * scale);
        }

        public static Color operator *(float scale, Color color)
        {
            return new Color(color.r * scale, color.g * scale, color.b * scale, color.a * scale);
        }

        public static Color operator /(Color a, Color b)
        {
            return new Color(a.r / b.r, a.g / b.g, a.b / b.b, a.a / b.a);
        }

        public static Color operator /(Color color, float scale)
        {
            float invS = 1.0f/scale;
            return new Color(color.r * invS, color.g * invS, color.b * invS, color.a * invS);
        }

        public static bool operator ==(Color a, Color b)
        {
            if (a.r - b.r > float.Epsilon)
                return false;
            if (b.r - a.r > float.Epsilon)
                return false;
            if (a.g - b.g > float.Epsilon)
                return false;
            if (b.g - a.g > float.Epsilon)
                return false;
            if (a.b - b.b > float.Epsilon)
                return false;
            if (b.b - a.b > float.Epsilon)
                return false;
            if (a.a - b.a > float.Epsilon)
                return false;
            if (b.a - a.a > float.Epsilon)
                return false;
            return true;
        }

        public static bool operator !=(Color a, Color b)
        {
            if (a.r - b.r > float.Epsilon)
                return true;
            if (b.r - a.r > float.Epsilon)
                return true;
            if (a.g - b.g > float.Epsilon)
                return true;
            if (b.g - a.g > float.Epsilon)
                return true;
            if (a.b - b.b > float.Epsilon)
                return true;
            if (b.b - a.b > float.Epsilon)
                return true;
            if (a.a - b.a > float.Epsilon)
                return true;
            if (b.a - a.a > float.Epsilon)
                return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            Color objcol = (Color) obj;
            return this == objcol;
        }

        public static Color Lerp(Color a, Color b, float t)
        {
            return new Color(a.r + (b.r - a.r)*t, a.g + (b.g - a.g)*t, a.b + (b.b - a.b)*t, a.a + (b.a - a.a)*t);
        }

        public static Color HSVToRGB(float h, float s, float v)
        {
            Color col = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            if (s >= 0.0f - float.Epsilon && s <= 0.0f + float.Epsilon)
            {
                col.r = v;
                col.g = v;
                col.b = v;
            }
            else if (v >= 0.0f - float.Epsilon && v <= 0.0f + float.Epsilon)
            {
                col.r = 0.0f;
                col.g = 0.0f;
                col.b = 0.0f;
            }
            else
            {
                col.r = 0.0f;
                col.g = 0.0f;
                col.b = 0.0f;
                float num = h*6.0f;
                int num2 = (int) Math.Floor(num);
                float num3 = num - (float) num2;
                float num4 = v*(1.0f - s);
                float num5 = v*(1.0f - s*num3);
                float num6 = v*(1.0f - s*(1.0f - num3));
                switch (num2 + 1)
                {
                    case 0:
                        col.r = v;
                        col.g = num4;
                        col.b = num5;
                        break;
                    case 1:
                        col.r = v;
                        col.g = num6;
                        col.b = num4;
                        break;
                    case 2:
                        col.r = num5;
                        col.g = v;
                        col.b = num4;
                        break;
                    case 3:
                        col.r = num4;
                        col.g = v;
                        col.b = num6;
                        break;
                    case 4:
                        col.r = num4;
                        col.g = num5;
                        col.b = v;
                        break;
                    case 5:
                        col.r = num6;
                        col.g = num4;
                        col.b = v;
                        break;
                    case 6:
                        col.r = v;
                        col.g = num4;
                        col.b = num5;
                        break;
                    case 7:
                        col.r = v;
                        col.g = num6;
                        col.b = num4;
                        break;
                }
            }
            return col;
        }

        public static Color Color32( ushort r, ushort g, ushort b, ushort a)
        {
            float red = ((float)r) / 255.0f;
            float green = ((float)g) / 255.0f;
            float blue = ((float)b) / 255.0f;
            float alpha = ((float)a) / 255.0f;
            return new Color(red, green, blue, alpha);
        }

        public static Color Color32(ushort r, ushort g, ushort b)
        {
            float red = ((float)r) / 255.0f;
            float green = ((float)g) / 255.0f;
            float blue = ((float)b) / 255.0f;
            return new Color(red, green, blue, 1.0f);
        }

        public static void RGBToHSV(Color color, ref float h, ref float s, ref float v)
        {
            if (color.b > color.g && color.b > color.r)
            {
                Color.RGBToHSV_Internal(4.0f, color.b, color.r, color.g, ref h, ref s, ref v);
            }
            else if (color.g > color.r)
            {
                Color.RGBToHSV_Internal(2.0f, color.g, color.b, color.r, ref h, ref s, ref v);
            }
            else
            {
                Color.RGBToHSV_Internal(0.0f, color.r, color.g, color.b, ref h, ref s, ref v);
            }
        }

        public static Color Parse(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new Color();
            string[] split = text.Split(',');

            float r = split.Length >= 1 ? float.Parse(split[0]) : 0.0f;
            float g = split.Length >= 2 ? float.Parse(split[1]) : 0.0f;
            float b = split.Length >= 3 ? float.Parse(split[2]) : 0.0f;
            float a = split.Length >= 4 ? float.Parse(split[3]) : 0.0f;

            return new Color(r, g, b, a);
        }

        public override string ToString()
        {
            return $"Color({this.r},{this.g},{this.b},{this.a})";
        }

        private static void RGBToHSV_Internal(float offset, float dominantcolor, float colorone, float colortwo, ref float h, ref float s, ref float v)
        {
            v = dominantcolor;
            if (v < 0.0f - float.Epsilon || v > 0.0f + float.Epsilon)
            {
                float num;
                if (colorone > colortwo)
                {
                    num = colortwo;
                }
                else
                {
                    num = colorone;
                }
                float num2 = v - num;
                if (num2 < 0.0f - float.Epsilon || num2 > 0.0f + float.Epsilon)
                {
                    s = num2 / v;
                    h = offset + (colorone - colortwo) / num2;
                }
                else
                {
                    s = 0.0f;
                    h = offset + (colorone - colortwo);
                }
                h /= 6.0f;
                if (h < 0.0f)
                {
                    h += 1.0f;
                }
            }
            else
            {
                s = 0.0f;
                h = 0.0f;
            }
        }
    }
}
