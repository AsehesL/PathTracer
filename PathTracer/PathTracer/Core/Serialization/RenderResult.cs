using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public interface IRenderResult
    {
        string GetExtensions();

        bool Save(string path);
    }
}
