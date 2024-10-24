Shader "Camera/AlphaControl"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _Alpha ("Alpha Value", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalRenderPipeline" }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha // ����͸���Ȼ��
            AlphaTest Greater 0.1 // ��ѡ������һ�� alpha ����ֵ������͸���Ȳü�

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct a2v
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _BaseMap;
            float _Alpha;
            float4 _BaseMap_ST;

            v2f vert(a2v v)
            {
                v2f o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // ������������
                float4 baseColor = tex2D(_BaseMap, i.uv);

                // Ӧ�� alpha ֵ������͸����
                baseColor.a *= _Alpha;

                // ����������ɫ
                return baseColor;
            }

            ENDHLSL
        }
    }
    FallBack Off
}
