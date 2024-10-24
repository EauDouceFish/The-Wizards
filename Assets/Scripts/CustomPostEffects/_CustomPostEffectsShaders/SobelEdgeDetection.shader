Shader "PostProcess/SobelEdgeDetection"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EdgesOnly ("Edges Only", Float) = 1.0
        _EdgeColor ("Edge Color", Color) = (0, 0, 0, 1)
        _BackgroundColor ("Background Color", Color) = (1, 1, 1, 1)
        _OutlineStrength("Outline Strength", Range(0.0, 10.0)) = 1.0
    }
    SubShader
    {
        ZTest Always
        Cull Off 
        ZWrite Off
        Tags { "RenderPipeline"="UniversalRenderPipeline" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct a2v
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv[9] : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            half4 _MainTex_TexelSize;
            float _EdgesOnly;
            half4 _EdgeColor;
            half4 _BackgroundColor;
            float _OutlineStrength;

            half luminance(half4 color)
            {
                return 0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b;
            }

            half Sobel(v2f i) 
            {
                const half Gx[9] = 
                {
                    -1, -2, -1,
                    0, 0, 0,
                    1, 2, 1
                };
                const half Gy[9] = 
                {
                    -1, 0, 1,
                    -2, 0, 2,
                    -1, 0, 1
                };

                half texColor;
                half edgeX = 0;
                half edgeY = 0;

                // 对每个像素周围的一共九个像素进行光照强度采样
                for(int it = 0; it < 9; it++){
                    texColor = luminance(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv[it]));
                    edgeX += texColor * Gx[it];
                    edgeY += texColor * Gy[it];
                }

                half edge = clamp(1.0 - (abs(edgeX) + abs(edgeY)) * _OutlineStrength, 0.0, 1.0);

                return edge;
            }

            v2f vert (a2v v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                half2 uv = v.texcoord;

                o.uv[0] = uv + _MainTex_TexelSize.xy * half2(-1,-1);
                o.uv[1] = uv + _MainTex_TexelSize.xy * half2(0,-1);
                o.uv[2] = uv + _MainTex_TexelSize.xy * half2(1,-1);
                o.uv[3] = uv + _MainTex_TexelSize.xy * half2(-1,0);
                o.uv[4] = uv + _MainTex_TexelSize.xy * half2(0,0);
                o.uv[5] = uv + _MainTex_TexelSize.xy * half2(1,0);
                o.uv[6] = uv + _MainTex_TexelSize.xy * half2(-1,1);
                o.uv[7] = uv + _MainTex_TexelSize.xy * half2(0,1);
                o.uv[8] = uv + _MainTex_TexelSize.xy * half2(1,1);
                
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half edge = Sobel(i);
                half4 withEdgeColor = lerp(_EdgeColor, _MainTex.Sample(sampler_MainTex, i.uv[4]), edge);
                half4 onlyEdgeColor = lerp(_EdgeColor, _BackgroundColor, edge);
                return lerp(withEdgeColor, onlyEdgeColor, _EdgesOnly);
            }
            ENDHLSL
        }
    }
    Fallback Off
}
