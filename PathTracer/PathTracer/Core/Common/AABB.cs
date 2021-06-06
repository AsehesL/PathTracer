using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public interface IAABB
    {
        Bounds GetBounds();
    }
}
