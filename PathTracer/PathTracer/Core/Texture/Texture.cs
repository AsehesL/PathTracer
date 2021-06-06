using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeImageAPI;

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
    public class Texture : IRenderResult
    {
        public uint width { get { return m_Width; } }
        public uint height { get { return m_Height; } }

        private uint m_Width;
        private uint m_Height;

        private float m_UDelta;
        private float m_VDelta;

        public WrapMode wrapMode;
        public FilterMode filterMode;

        private Color[] m_Colors;

        public Texture(uint width, uint height)
        {
            m_Width = width;
            m_Height = height;

            m_UDelta = 1.0f / width;
            m_VDelta = 1.0f / height;

            m_Colors = new Color[width * height];
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

        public static Texture Create(string path, float gamma)
        {
            if (!System.IO.File.Exists(path))
                return null;
            if (!FreeImage.IsAvailable())
                return null;

            FIBITMAP dib = new FIBITMAP();
            dib = FreeImage.LoadEx(path);
            if (dib.IsNull)
                return null;
            var bpp = FreeImage.GetBPP(dib);
            if (bpp != 24 && bpp != 32 && bpp != 96 && bpp != 8)
            {
                FreeImage.UnloadEx(ref dib);
                return null;
            }
            uint w = FreeImage.GetWidth(dib);
            uint h = FreeImage.GetHeight(dib);

            Texture tex = new Texture(w, h);
            for (int i = 0; i < h; i++)
            {
                if(bpp == 8)
                {
                    Scanline<Byte> scanline = new Scanline<Byte>(dib, i);

                    Byte[] data = scanline.Data;
                    for (int j = 0; j < data.Length; j++)
                    {
                        var gray = data[j];
                        Color color = Color.Color32(gray, gray, gray);
                        color.Gamma(gamma);
                        tex.SetPixel(j, i, color);
                    }
                }
                else if (bpp == 24)
                {
                    Scanline<RGBTRIPLE> scanline = new Scanline<RGBTRIPLE>(dib, i);

                    RGBTRIPLE[] data = scanline.Data;
                    for (int j = 0; j < data.Length; j++)
                    {
                        var red = data[j].rgbtRed;
                        var green = data[j].rgbtGreen;
                        var blue = data[j].rgbtBlue;
                        Color color = Color.Color32(red, green, blue);
                        color.Gamma(gamma);
                        tex.SetPixel(j, i, color);
                    }
                }
                else if (bpp == 32)
                {
                    Scanline<RGBQUAD> scanline = new Scanline<RGBQUAD>(dib, i);

                    RGBQUAD[] data = scanline.Data;

                    for (int j = 0; j < data.Length; j++)
                    {
                        var red = data[j].rgbRed;
                        var green = data[j].rgbGreen;
                        var blue = data[j].rgbBlue;
                        var alpha = data[j].rgbReserved;
                        Color color = Color.Color32(red, green, blue, alpha);
                        color.Gamma(gamma);
                        tex.SetPixel(j, i, color);
                    }
                }
                else if (bpp == 96)
                {
                    Scanline<FIRGBF> scanline = new Scanline<FIRGBF>(dib, i);

                    FIRGBF[] data = scanline.Data;

                    for (int j = 0; j < data.Length; j++)
                    {
                        var red = data[j].red;
                        var green = data[j].green;
                        var blue = data[j].blue;
                        Color color = new Color(red, green, blue);
                        tex.SetPixel(j, i, color);
                    }
                }
            }

            return tex;
        }

        public void SetPixel(int x, int y, Color color)
        {
            m_Colors[y * m_Width + x] = color;
        }

        public Color GetPixel(int x, int y)
        {
            if (wrapMode == WrapMode.Clamp)
            {
                if (x < 0)
                    x = 0;
                else if (x >= m_Width)
                    x = (int) m_Width - 1;
                if (y < 0)
                    y = 0;
                else if (y >= m_Height)
                    y = (int) m_Height - 1;
            }
            else if (wrapMode == WrapMode.Repeat)
            {
                int w = (int) m_Width;
                int h = (int) m_Height;
                if (x < 0)
                    x = w - 1 + x % w;
                else if (x >= w)
                    x = x % w;
                if (y < 0)
                    y = h - 1 + y % h;
                else if (y >= h)
                    y = y % h;
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

        public string GetExtensions()
        {
            return "JPEG文件|*.jpg|BMP文件|*.bmp|PNG文件|*.png|HDR文件|*.hdr";
        }

        public bool Save(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            bool isHDR = false;
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(path);
            if (fileInfo.Extension.ToLower().Contains(".hdr"))
            {
                isHDR = true;
            }

            FIBITMAP dib = new FIBITMAP();
            if (isHDR)
            {
                dib = FreeImage.AllocateT(FREE_IMAGE_TYPE.FIT_RGBF, (int)width, (int)height, 96);

                uint h = FreeImage.GetHeight(dib);

                for (int i = 0; i < h; i++)
                {
                    Scanline<FIRGBF> scanline = new Scanline<FIRGBF>(dib, i);

                    FIRGBF[] data = scanline.Data;

                    for (int j = 0; j < data.Length; j++)
                    {
                        Color color = this.GetPixel(data.Length - 1 - j, i);
                        data[j].red = color.r;
                        data[j].green = color.g;
                        data[j].blue = color.b;
                    }

                    scanline.Data = data;
                }
            }
            else
            {
                FREE_IMAGE_FORMAT fif = FreeImage.GetFIFFromFilename(path);
                dib = FreeImage.AllocateT(FREE_IMAGE_TYPE.FIT_BITMAP, (int)width, (int)height, 24);

                uint h = FreeImage.GetHeight(dib);
                for (int i = 0; i < h; i++)
                {
                    Scanline<RGBTRIPLE> scanline = new Scanline<RGBTRIPLE>(dib, i);

                    RGBTRIPLE[] data = scanline.Data;

                    for (int j = 0; j < data.Length; j++)
                    {
                        Color color = this.GetPixel(data.Length - 1 - j, i);
                        color.Gamma(0.45f);

                        int r = (int)(color.r * 255.0f);
                        int g = (int)(color.g * 255.0f);
                        int b = (int)(color.b * 255.0f);
                        if (r < 0) r = 0;
                        if (r > 255) r = 255;
                        if (g < 0) g = 0;
                        if (g > 255) g = 255;
                        if (b < 0) b = 0;
                        if (b > 255) b = 255;

                        data[j].rgbtRed = (byte)r;
                        data[j].rgbtGreen = (byte)g;
                        data[j].rgbtBlue = (byte)b;
                    }

                    scanline.Data = data;
                }
            }

            if (!FreeImage.SaveEx(ref dib, path, true))
            {
                FreeImage.UnloadEx(ref dib);
                return false;
            }
            FreeImage.UnloadEx(ref dib);
            return true;
        }
    }
}
