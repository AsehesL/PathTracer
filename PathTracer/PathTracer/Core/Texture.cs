using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public class Texture
    {
        public int width { get { return m_Width; } }
        public int height { get { return m_Height; } }

        private int m_Width;
        private int m_Height;

        private System.Object m_Lock;

        private Color[] m_Colors;

        public Texture(int width, int height)
        {
            m_Width = width;
            m_Height = height;

            m_Colors = new Color[width * height];

            m_Lock = new object();
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
                    tex.SetPixel(img.Width-1-i,img.Height-1-j, col);
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
            lock (m_Lock)
            {
                return m_Colors[y * m_Width + x];
            }
        }

        public System.Drawing.Image SaveToImage()
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
