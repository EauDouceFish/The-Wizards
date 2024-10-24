Shader "MyShaders/Stippling"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _NearClip("NearClip", Range(0, 5)) = 1.0
        _FarClip("FarClip", Range(0, 30)) = 3.0
        _Range ("Range", Float) = 1.0 
        _ManualControlTest ("ManualControlTest", Range(0, 10)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            

            struct appdata_img
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            float4 _Color;
            float _Range;
            float _NearClip;
            float _FarClip;

            float _ManualControlTest;

            v2f vert (appdata_img v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.screenPos = ComputeScreenPos(o.pos);
                o.worldPos = TransformObjectToWorld(v.vertex.xyz);
                return o;
            }



            half4 frag (v2f i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                // 重要步骤：计算和屏幕空间的距离,利用参数将其缩放在0-1范围
                float distanceRamp = distance(i.worldPos, _WorldSpaceCameraPos.xyz);
                distanceRamp = smoothstep(_NearClip, _FarClip, distanceRamp);

                // 和像素总数相乘，进行逐像素操作
                float2 pixelPos = i.screenPos.xy / i.screenPos.w * _ScreenParams.xy;

                const float4x4 thresholdMatrix =
                {
                    1.0, 9.0, 3.0, 11.0,
                    13.0, 5.0, 15.0, 7.0,
                    4.0, 12.0, 2.0, 10.0,
                    16.0, 8.0, 14.0, 6.0
                };

                half threshold = thresholdMatrix[floor(fmod(pixelPos.x, 4))][floor(fmod(pixelPos.y, 4))] / 17.0;
                
                clip(distanceRamp * _Range - threshold);

                return col;
            }
            ENDHLSL
        }
    }
}
