using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public enum WrapMode
    {
        Repeat,
        Clamp,
    }

    public enum FilterMode
    {
        Point,
        /// <summary>
        /// 双线性插值
        /// </summary>
        Bilinear,
    }
    public class Texture
    {
        public int width { get { return m_Width; } }
        public int height { get { return m_Height; } }

        private int m_Width;
        private int m_Height;

        private float m_UDelta;
        private float m_VDelta;

        public WrapMode wrapMode;
        public FilterMode filterMode;

        private System.Object m_Lock;

        private Color[] m_Colors;

        public Texture(int width, int height)
        {
            m_Width = width;
            m_Height = height;

            m_UDelta = 1.0f / width;
            m_VDelta = 1.0f / height;

            m_Colors = new Color[width * height];

            m_Lock = new object();
        }

        public void Fill(Color color)
        {
            for (int i = 0; i < m_Width; i++)
            {
                for (int j = 0; j < m_Height; j++)
                {
                    m_Colors[j * m_Width + i] = color;
                }
            }
        }

        public static Texture Create(string path)
        {
            if (!System.IO.File.Exists(path))
                return null;
            var img = new System.Drawing.Bitmap(Image.FromFile(path));

            Texture tex = new Texture(img.Width, img.Height);
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    var c = img.GetPixel(i, j);
                    Color col = new Color(((float) c.R) / 255.0f, ((float) c.G) / 255.0f, ((float) c.B) / 255.0f,
                        ((float) c.A) / 255.0f);
                    tex.SetPixel(i, img.Height - 1 - j, col);
                }
            }

            return tex;
        }

        public void SetPixel(int x, int y, Color color)
        {
            lock (m_Lock)
            {
                m_Colors[y * m_Width + x] = color;
            }
        }

        public Color GetPixel(int x, int y)
        {
            if (wrapMode == WrapMode.Clamp)
            {
                if (x < 0)
                    x = 0;
                else if (x >= m_Width)
                    x = m_Width - 1;
                if (y < 0)
                    y = 0;
                else if (y >= m_Height)
                    y = m_Height - 1;
            }
            else if (wrapMode == WrapMode.Repeat)
            {
                if (x < 0)
                    x = m_Width + x % m_Width;
                else if (x >= m_Width)
                    x = x % m_Width;
                if (y < 0)
                    y = m_Height + y % m_Height;
                else if (y >= m_Height)
                    y = y % m_Height;
            }
            return m_Colors[y * m_Width + x];
        }

        public Color Sample(float u, float v)
        {
            //v = 1.0f - v;
            if (filterMode == FilterMode.Bilinear)
            {
                int cellx = (int) Math.Floor(u / m_UDelta);
                int celly = (int) Math.Floor(v / m_VDelta);

                float cx = (u - m_UDelta * cellx) / m_UDelta;
                float cy = (v - m_VDelta * celly) / m_VDelta;

                Color topleft = GetPixel(cellx, celly);
                Color topright = GetPixel(cellx + 1, celly);
                Color bottomleft = GetPixel(cellx, celly + 1);
                Color bottomright = GetPixel(cellx + 1, celly + 1);

                Color lerp = Color.Lerp(Color.Lerp(topleft, topright, cx), Color.Lerp(bottomleft, bottomright, cx), cy);
                return lerp;
            }
            else if (filterMode == FilterMode.Point)
            {
                int x = (int)Math.Floor(u * m_Width);
                int y = (int)Math.Floor(v * m_Height);
                return GetPixel(x, y);
            }

            return default(Color);
        }

        public System.Drawing.Bitmap SaveToImage()
        {
            var img = new System.Drawing.Bitmap(m_Width, m_Height);
            for (int i = 0; i < m_Width; i++)
            {
                for (int j = 0; j < m_Height; j++)
                {
                    Color col = m_Colors[j * m_Width + i];
                    col.FixColor();
                    System.Drawing.Color c = System.Drawing.Color.FromArgb((int) (col.r * 255.0f),
                        (int) (col.g * 255.0f), (int) (col.b * 255.0f));
                    img.SetPixel(m_Width - 1 - i, m_Height - 1 - j, c);
                }
            }

            return img;
        }
    }
}
