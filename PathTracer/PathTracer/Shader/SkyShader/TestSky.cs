using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    [ShaderType("TestSky")]
    class TestSky : Sky
    {
        private Vector3 m_SunDir = new Vector3(-1.0f, -0.8f, -1.0f);

        public override Color RenderColor(Vector3 dir)
        {
            Vector3 sundir = -1*m_SunDir.normalized;
            Color col = default(Color);

            float sign = 0 > dir.y ? 0 : 1;

            float hort = 1f - (float)MathUtils.Clamp(Math.Abs(dir.y), 0.0f, 1.0f);

            Color skyC = Color.Lerp(new Color(0.302f, 0.38f, 0.537f), new Color(0.435f, 0.545f, 0.702f), (float)Math.Pow(2, hort * 1.4 - 1.4));
            Color groundC = Color.Lerp(new Color(0.412f, 0.384f, 0.365f), new Color(0.435f, 0.545f, 0.702f), (float)Math.Pow(2, hort * 4.4f * 2.7f - 4.4f * 2.7f));
            col += Color.Lerp(groundC, skyC, sign);

            col += 0.3f * new Color(0.8f, 0.9f, 1.0f) * (float)Math.Pow(2, hort * 20.0f - 20.0f);
            col += 0.1f * new Color(0.8f, 0.9f, 1.0f) * (float)Math.Pow(2, hort * 15.0f - 15.0f);

            float sun = (float)MathUtils.Clamp(Vector3.Dot(sundir, dir), 0.0f, 1.0f);
            col += 0.2f * new Color(1.0f, 0.8f, 0.2f) * (float)Math.Pow(sun, 2.0f);
            col += 0.5f * new Color(1.0f, 0.8f, 0.9f) * (float)Math.Pow(2, sun * 650.0f - 650.0f);
            col += 0.1f * new Color(1.0f, 1.0f, 0.8f) * (float)Math.Pow(2, sun * 100.0f - 100.0f);
            col += 0.3f * new Color(1.0f, 0.8f, 0.8f) * (float)Math.Pow(2, sun * 50.0f - 50.0f);
            col += 0.5f * new Color(0.7f, 0.3f, 0.05f) * (float)Math.Pow(2, sun * 10.0f - 10.0f);

            col.a = 1;
            return col;
        }
    }
}
