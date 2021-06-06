using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public class VolumeTexture : IRenderResult
    {
        public uint width { get; private set; }
        public uint height { get; private set; }
        public uint depth { get; private set; }

        private float m_UDelta;
        private float m_VDelta;
        private float m_WDelta;

        public WrapMode wrapMode;
        public FilterMode filterMode;

        private Color[] m_Colors;

        public VolumeTexture(uint width, uint height, uint depth)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;

            m_UDelta = 1.0f / width;
            m_VDelta = 1.0f / height;
            m_WDelta = 1.0f / depth;

            m_Colors = new Color[width * height * depth];
        }

        public void Fill(Color color)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < depth; k++)
                    {
                        m_Colors[k * width * height + j * width + i] = color;
                    }
                }
            }
        }

        public void SetVoxel(int x, int y, int z, Color color)
        {
            m_Colors[z * width * height + y * width + x] = color;
        }

        public Color GetVoxel(int x, int y, int z)
        {
            if (wrapMode == WrapMode.Clamp)
            {
                if (x < 0)
                    x = 0;
                else if (x >= width)
                    x = (int)width - 1;
                if (y < 0)
                    y = 0;
                else if (y >= height)
                    y = (int)height - 1;
                if (z < 0)
                    z = 0;
                else if (z >= depth)
                    z = (int)depth - 1;
            }
            else if (wrapMode == WrapMode.Repeat)
            {
                int w = (int)width;
                int h = (int)height;
                int d = (int)depth;
                if (x < 0)
                    x = w - 1 + x % w;
                else if (x >= w)
                    x = x % w;
                if (y < 0)
                    y = h - 1 + y % h;
                else if (y >= h)
                    y = y % h;
                if (z < 0)
                    z = d - 1 + z % d;
                else if (z >= d)
                    z = z % d;
            }
            return m_Colors[z * width * height + y * width + x];
        }

        public Color Sample(float u, float v, float w)
        {
            //v = 1.0f - v;
            if (filterMode == FilterMode.Bilinear)
            {
                int cellx = (int)Math.Floor(u / m_UDelta);
                int celly = (int)Math.Floor(v / m_VDelta);
                int cellz = (int)Math.Floor(w / m_WDelta);

                float cx = (u - m_UDelta * cellx) / m_UDelta;
                float cy = (v - m_VDelta * celly) / m_VDelta;
                float cz = (w - m_WDelta * cellz) / m_WDelta;

                Color topleftforward = GetVoxel(cellx, celly, cellz);
                Color toprightforward = GetVoxel(cellx + 1, celly, cellz);
                Color bottomleftforward = GetVoxel(cellx, celly + 1, cellz);
                Color bottomrightforward = GetVoxel(cellx + 1, celly + 1, cellz);

                Color topleftback = GetVoxel(cellx, celly, cellz + 1);
                Color toprightback = GetVoxel(cellx + 1, celly, cellz + 1);
                Color bottomleftback = GetVoxel(cellx, celly + 1, cellz + 1);
                Color bottomrightback = GetVoxel(cellx + 1, celly + 1, cellz + 1);

                Color lerpForward = Color.Lerp(Color.Lerp(topleftforward, toprightforward, cx), Color.Lerp(bottomleftforward, bottomrightforward, cx), cy);
                Color lerpBack = Color.Lerp(Color.Lerp(topleftback, toprightback, cx), Color.Lerp(bottomleftback, bottomrightback, cx), cy);
                Color lerp = Color.Lerp(lerpForward, lerpBack, cz);
                return lerp;
            }
            else if (filterMode == FilterMode.Point)
            {
                int x = (int)Math.Floor(u * width);
                int y = (int)Math.Floor(v * height);
                int z = (int)Math.Floor(w * depth);
                return GetVoxel(x, y, z);
            }

            return default(Color);
        }

        public string GetExtensions()
        {
            return "VolumeTexture文件|*.vxtex";
        }

        public bool Save(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            
            return true;
        }
    }
}
