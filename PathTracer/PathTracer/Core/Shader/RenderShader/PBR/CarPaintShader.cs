using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
	struct CarPaintProperty : IPBRProperty
	{
		public Color albedo;
		public float roughness;
		public float roughness2;
		public Color emissive;
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
			return roughness2;
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
			return 0.0f;
		}
		public Color GetDoubleSidedTransmissionColor()
		{
			return Color.black;
		}
	}

	[ShaderType("CarPaint")]
	class CarPaintShader : PBRShaderBase<CarPaintProperty>
    {
		public Color color;
		public Texture albedo;
		public Texture rroTex;
		public float roughness;
		public float roughness2;
		public Color emissive;
		public Texture emissiveTex;
		public Vector2 tile;
		public Texture bump;

		public override PBRShadingModel shadingModel => PBRShadingModel.CarPaint;

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

		protected override CarPaintProperty SampleProperty(Ray ray, RayCastHit hit)
		{
			CarPaintProperty property = default(CarPaintProperty);
			property.albedo = color;
			if (albedo != null)
			{
				property.albedo *= albedo.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y));
			}
			Color rro = rroTex != null ? rroTex.Sample((float)(hit.texcoord.x * tile.x), (float)(hit.texcoord.y * tile.y)) : Color.black;
			if (rroTex != null)
			{
				property.roughness = rro.r;
				property.roughness2 = rro.g;
			}
			else
			{
				property.roughness = roughness;
				property.roughness2 = roughness2;
			}
			
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
			if (rroTex != null)
				property.occlusion = rro.b;
			else
				property.occlusion = 1.0f;
			return property;
		}
	}
}