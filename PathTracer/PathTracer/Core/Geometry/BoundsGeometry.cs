﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public abstract class BoundsGeometry : Geometry
    {

        public Bounds bounds;

        public BoundsGeometry(Shader shader) : base(shader)
        {
        }
    }
}