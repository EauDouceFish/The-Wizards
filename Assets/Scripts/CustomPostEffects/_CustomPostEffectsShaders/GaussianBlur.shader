Shader "PostProcess/GaussianBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        HLSLINCLUDE
        
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        half4 _MainTex_TexelSize;
		float _BlurSize;

        struct appdata_img
        {
            float4 vertex : POSITION;
            float2 texcoord : TEXCOORD0;
        };

        struct v2f{
            float4 pos : SV_POSITION;
            half2 uv[5] : TEXCOORD0;
        };

        // 直接对最后图像处理，不需要顶点数据
        // 另：对于5*5的高斯模糊，根据对称性只需两个编号为12321 * 12321的数组相乘即可，则只需要123三个权重
        v2f vertBlurVertical(appdata_img v)
        {
            v2f o;
            o.pos = TransformObjectToHClip(v.vertex.xyz);
            half2 uv = v.texcoord;
			
			o.uv[0] = uv;
			o.uv[1] = uv + float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
			o.uv[2] = uv - float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
			o.uv[3] = uv + float2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
			o.uv[4] = uv - float2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;

            return o;
        }

        v2f vertBlurHorizontal(appdata_img v)
        {
            v2f o;

            o.pos = TransformObjectToHClip(v.vertex.xyz);
            half2 uv = v.texcoord;
			o.uv[0] = uv;
			o.uv[1] = uv + float2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
			o.uv[2] = uv - float2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
			o.uv[3] = uv + float2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
			o.uv[4] = uv - float2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
					 
			return o;
        }
        		
		half4 fragBlur(v2f i) : SV_Target {

            // 记录所需要的三个权重3 2 1
			float weight[3] = {0.4026, 0.2442, 0.0545};
			
			half3 sum = _MainTex.Sample(sampler_MainTex, i.uv[0]).rgb * weight[0];
			
			for (int it = 1; it < 3; it++) {
				sum += _MainTex.Sample(sampler_MainTex, i.uv[it*2-1]).rgb * weight[it];
				sum += _MainTex.Sample(sampler_MainTex, i.uv[it*2]).rgb * weight[it];
			}
			
			return half4(sum, 1.0);
		}

        ENDHLSL

        // 后处理必要措施
        ZTest Always Cull Off ZWrite Off

		Pass {
			NAME "GAUSSIAN_BLUR_VERTICAL"
			
			HLSLPROGRAM
			  
			#pragma vertex vertBlurVertical  
			#pragma fragment fragBlur
			  
			ENDHLSL  
		}
		
		Pass {  
			NAME "GAUSSIAN_BLUR_HORIZONTAL"
			
			HLSLPROGRAM  
			
			#pragma vertex vertBlurHorizontal  
			#pragma fragment fragBlur
			
			ENDHLSL
		}
    }
}
