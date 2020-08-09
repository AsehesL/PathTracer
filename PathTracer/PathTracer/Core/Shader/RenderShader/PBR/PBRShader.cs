using System;
using System.Collections.Generic;
using System.Windows.Forms.Layout;

namespace ASL.PathTracer
{
	struct PBRProperty : IPBRProperty
	{
		public Color albedo;
		public float roughness;
		public float metallic;
		public Color emissive;
		public float refractive;
		public bool tangentSpaceNormal;
		public Color normal;
		public float occlusion;

		public Color GetAlbedo()
		{
			return albedo;
		}
		public float GetRoughness()
		{
			return roughness;
		}
		public float GetRoughness2()
		{
			return 0.0f;
		}
		public float GetMetallic()
		{
			return metallic;
		}
		public Color GetEmissive()
		{
			return emissive;
		}
		public float GetRefractive()
		{
			return refractive;
		}
		public bool TangentSpaceNormal()
		{
			return tangentSpaceNormal;
		}
		public Color GetNormal()
		{
			return normal;
		}
		public float GetOcclusion()
		{
			return occlusion;
		}
		public float GetDoubleSidedTransmittance()
		{
			return 0.0f;
		}
		public Color GetDoubleSidedTransmissionColor()
		{
			return Color.black;
		}
	}

	[ShaderType("PBR")]
    class PBRShader : PBRShaderBase<PBRProperty>
    {

		public Color color;
        public Texture albedo;
		public float metallic;
		public float roughness;
		public Color emissive;
		public Texture emissiveTex;
		public float refractive;
		public Vector2 tile;
		public Texture bump;
		public bool transparent;
		public Texture mroTex;

		public override PBRShadingModel shadingModel
		{
			get
			{
				if (transparent)
					return PBRShadingModel.Transparency;
				else
					return PBRShadingModel.Default;
			}
		}

		protected override BRDFBase DiffuseBRDF
		{
			get { return m_LambertaianBRDF; }
		}

		protected override BRDFBase SpecularBRDF
		{
			get { return m_CookTorranceBRDF; }
		}

		private LambertatianBRDF m_LambertaianBRDF = new LambertatianBRDF();

		private CookTorranceBRDF m_CookTorranceBRDF = new CookTorranceBRDF();

		protected override float GetTransparentCutOutAlpha(Ray ray, RayCastHit hit)
		{
			float alpha = color.a;
			if (albedo != null)
			{
				alpha *= albedo.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y)).a;
			}
			return alpha;
		}

		protected override PBRProperty SampleProperty(Ray ray, RayCastHit hit)
		{
			PBRProperty property = default(PBRProperty);
			property.albedo = color;
			if (albedo != null)
			{
				property.albedo *= albedo.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y));
			}
			Color mro = mroTex != null ? mroTex.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y)) : Color.black;
			if (mroTex != null)
			{
				property.roughness = mro.g;
			}
			else
				property.roughness = roughness;
			if (shadingModel != PBRShadingModel.Transparency)
			{
				if (mroTex != null)
				{
					property.metallic = mro.r;
				}
				else
					property.metallic = metallic;
			}
			property.refractive = refractive;

			property.emissive = emissive;
			if (emissiveTex != null)
			{
				property.emissive *= emissiveTex.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y));
			}

			if (bump == null)
				property.tangentSpaceNormal = false;
			else
			{
				property.tangentSpaceNormal = true;
				property.normal = bump.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y));
			}
			//if (shadingModel != PBRShadingModel.Transparent)
			//{
			//	if (metallicSmooth != null)
			//	{
			//		Color col = metallicSmooth.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y));
			//		property.metallic = col.r;
			//		property.roughness = (1.0f - col.a);
			//	}
			//}
			if (mroTex != null)
				property.occlusion = mro.b;
			else
				property.occlusion = 1.0f;
			return property;
		}
	}
}
