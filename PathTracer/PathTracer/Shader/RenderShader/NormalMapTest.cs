﻿using System;
using System.Collections.Generic;

namespace ASL.PathTracer
{
	[ShaderType("NTest")]
	class NormalMapTest : Shader
	{
		public Texture bump;

		public override Color Render(Tracer tracer, Sky sky, SamplerBase sampler, Ray ray, RayCastHit hit, double epsilon)
		{
			Color bmpcol = bump.Sample((float)(hit.texcoord.x), (float)(hit.texcoord.y));

			Vector3 wbnormal = Vector3.Cross(hit.normal, hit.tangent).normalized;

			Vector3 rnormal = new Vector3(bmpcol.r * 2 - 1, bmpcol.g * 2 - 1, bmpcol.b * 2 - 1);
			Vector3 worldnormal = default(Vector3);
			worldnormal.x = hit.tangent.x * rnormal.x + wbnormal.x * rnormal.y + hit.normal.x * rnormal.z;
			worldnormal.y = hit.tangent.y * rnormal.x + wbnormal.y * rnormal.y + hit.normal.y * rnormal.z;
			worldnormal.z = hit.tangent.z * rnormal.x + wbnormal.z * rnormal.y + hit.normal.z * rnormal.z;
			worldnormal.Normalize();
			if (Vector3.Dot(worldnormal, hit.normal) < 0)
				worldnormal *= -1;


			Vector3 w = Vector3.Reflect(ray.direction.normalized * -1, worldnormal);
			//Vector3 w = Vector3.Reflect(ray.direction.normalized * -1, hit.normal);

			Ray lray = new Ray(hit.hit, w);
			return tracer.Tracing(lray, sky, sampler, hit.depth + 1);
		}

		public override Color FastRender(Ray ray, RayCastHit hit)
		{
			return Color.black;
		}
	}
}