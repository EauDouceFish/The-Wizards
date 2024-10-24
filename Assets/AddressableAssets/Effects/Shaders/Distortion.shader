Shader "MyShaders/DistortBlendCustom" {
	Properties {
		_Brightness("Brightness", float) = 1
		_Contrast("Contrast", Float) = 1
		_MainColor("Main Color", Color) = (1,1,1,1)
		_MainTex("Main Tex (A)", 2D) = "white" {}
		_MainPannerX("Main Panner X", float) = 0
		_MainPannerY("Main Panner Y", float) = 0
		_TurbulenceTex("Turbulence Tex", 2D) = "white" {}
		_MaskTex("Mask Tex", 2D) = "white" {}
		_CubeMap("CubeMap", Cube) = "" {}
		Power("Distort Power", float) = 0
		_PowerX("Power X", range(0,1)) = 0
		_PowerY("Power Y", range(0,1)) = 0
	}

	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZWrite Off

		Pass {
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			struct a2v {
				float4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				half2 uvMain : TEXCOORD1;
				half2 uvNoise : TEXCOORD2;
				half2 uvMask : TEXCOORD3;
				float4 color : COLOR;
			};

			float _Brightness;
			float _Contrast;
			float4 _MainColor;
			float _PowerX;
			float _PowerY;
			float _DistortPower;
			float4 _MainTex_ST;
			float4 _TurbulenceTex_ST;
			float4 _MaskTex_ST;
			float _MainPannerX;
			float _MainPannerY;

			TEXTURE2D(_MainTex);
			TEXTURE2D(_TurbulenceTex);
			TEXTURE2D(_MaskTex);
			TEXTURECUBE(_CubeMap);
			SAMPLER(sampler_MainTex);
			SAMPLER(sampler_TurbulenceTex);
			SAMPLER(sampler_MaskTex);
			SAMPLER(sampler_CubeMap);

			float4 _CubeMap_HDR;

			v2f vert(a2v v) {
				v2f o;
				o.pos = TransformObjectToHClip(v.vertex);
				o.uvMain = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uvNoise = TRANSFORM_TEX(v.texcoord, _TurbulenceTex);
				o.uvMask = TRANSFORM_TEX(v.texcoord, _MaskTex);
				o.color = v.color;
				return o;
			}

			float4 frag(v2f i) : SV_Target {
				float4 offsetColor1 = SAMPLE_TEXTURE2D(_TurbulenceTex, sampler_TurbulenceTex, i.uvNoise + fmod(_Time.xz * _DistortPower, 1));
				float4 offsetColor2 = SAMPLE_TEXTURE2D(_TurbulenceTex, sampler_TurbulenceTex, i.uvNoise + fmod(_Time.yx * _DistortPower, 1));

				float4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uvMask);

				half2 oldUV = frac(i.uvMain);

				i.uvMain.x += ((offsetColor1.r + offsetColor2.r) - 1) * _PowerX;
				i.uvMain.y += ((offsetColor1.r + offsetColor2.r) - 1) * _PowerY;

				half2 resUV = lerp(oldUV, i.uvMain, mask.xy);

				// Panner is the speed
				resUV.x += fmod(_MainPannerX * _Time.y, 1);
				resUV.y += fmod(_MainPannerY * _Time.y, 1);

				float4 _MainTexVar = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, resUV);
				float4 _MaskTexVar = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uvMask);

				float3 emissive = _MainColor.rgb * _Brightness * pow(_MainTexVar.rgb, _Contrast) * i.color.rgb;
				float finalAlpha = _MainColor.a * _MainTexVar.a * i.color.a * _MaskTexVar.r;

				return float4(emissive, finalAlpha);
			}

			ENDHLSL
		}
	}
}
