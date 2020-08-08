using ASL.PathTracer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
	struct DoubleSidedTransmittanceProperty : IPBRProperty
	{
		public Color albedo;
		public float roughness;
		public Color emissive;
		public bool tangentSpaceNormal;
		public Color normal;
		public float occlusion;
		public float transmittance;
		public Color transmissionColor;

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
			return 0.0f;
		}
		public Color GetEmissive()
		{
			return emissive;
		}
		public float GetRefractive()
		{
			return 0.0f;
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
			return transmittance;
		}
		public Color GetDoubleSidedTransmissionColor()
		{
			return transmissionColor;
		}
	}

	[ShaderType("Transmittance")]
	class DoubleSidedTransmittanceShader : PBRShaderBase <DoubleSidedTransmittanceProperty>
    {
		public Color color;
		public Texture albedo;
		public Texture roughnessTex;
		public float roughness;
		public Color emissive;
		public Texture emissiveTex;
		public Vector2 tile;
		public Texture bump;
		public Texture aoTex;
		public Color transmissionColor;
		public Texture transmissionTex;
		public float transmittance;

		public override PBRShadingModel shadingModel => PBRShadingModel.DoubleSidedTranslucent;

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

		protected override DoubleSidedTransmittanceProperty SampleProperty(Ray ray, RayCastHit hit)
		{
			DoubleSidedTransmittanceProperty property = default(DoubleSidedTransmittanceProperty);
			property.albedo = color;
			if (albedo != null)
			{
				property.albedo *= albedo.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y));
			}
			if (roughnessTex != null)
			{
				property.roughness = roughnessTex.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y)).r;
			}
			else
				property.roughness = roughness;
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
			if (aoTex != null)
				property.occlusion = aoTex.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y)).r;
			else
				property.occlusion = 1.0f;
			property.transmissionColor = transmissionColor;
			if (transmissionTex != null)
			{
				property.transmissionColor *= transmissionTex.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y));
			}
			property.transmittance = transmittance;
			return property;
		}
	}
}
