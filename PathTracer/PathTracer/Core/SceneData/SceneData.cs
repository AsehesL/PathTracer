using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public abstract class SceneData
    {
        public abstract void Build(List<Geometry> geometries);

        public abstract bool Raycast(Ray ray, double epsilon, out RayCastHit hit);
    }
}
