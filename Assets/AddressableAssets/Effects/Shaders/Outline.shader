Shader "MyShaders/Outline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor("OutlineColor", Color) = (1,0,0,1)  
        _OutlineFactor("OutlineFactor", Range(1,1.1)) = 1.03
    }
    SubShader
    {
    cull front
        Tags { "RenderType"="Opaque"}
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            float _OutlineFactor;  
            float4 _OutlineColor;  
            struct a2v
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (a2v v)
            {
                v2f o;
                v.vertex.xyz *= _OutlineFactor;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}