using ASL.PathTracer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PathTracerForm
{
    public static class TextureExtend
    {
        public static System.Drawing.Bitmap TransferToBMP(this ASL.PathTracer.Texture texture, System.Drawing.Bitmap bitmap, float gamma, float exposure)
        {
            if (bitmap == null)
                bitmap = new System.Drawing.Bitmap((int)texture.width, (int)texture.height);
            for (int i = 0; i < texture.width; i++)
            {
                for (int j = 0; j < texture.height; j++)
                {
                    Color col = texture.GetPixel(i, j);
                    if (exposure > 0)
                        col.Tonemapping(exposure);
                    col.Gamma(gamma);
                    //col.FixColor(gamma);
                    col.ClampColor();
                    System.Drawing.Color c = System.Drawing.Color.FromArgb((int)(col.r * 255.0f),
                        (int)(col.g * 255.0f), (int)(col.b * 255.0f));
                    bitmap.SetPixel((int)texture.width - 1 - i, (int)texture.height - 1 - j, c);
                }
            }

            return bitmap;
        }
    }
}
