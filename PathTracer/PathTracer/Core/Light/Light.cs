using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public abstract class Light
    {
        public abstract Vector3 Sample(SamplerBase sampler, Vector3 hitPoint);

        public abstract Vector3 GetNormal(Vector3 hitPoint, Vector3 surfacePoint);

        public abstract float GetPDF();

        public abstract bool L(SceneData sceneData, Ray ray, out Color color, out Vector3 surfaceNormal);

        public virtual float G(Ray ray, Vector3 surfacePoint, Vector3 surfaceNormal) { return 1.0f; }
    }
}
