// uv0 is the main uv
// uv1 is the detail uv, r g including Distort and Dissolve
// while b a is the tunnel for another uv

Shader "URP/Add_T"
{
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        [MaterialToggle] _MainTexAlphaOn ("MainTexAlpha On", Float ) = 0
        [HDR]_Color ("Color", Color) = (0.5,0.5,0.5,1)

        [MaterialToggle] _PanUVOn ("PanUV On", Float ) = 1
        _UPanSpeed ("U Pan Speed", Float ) = 0
        _VPanSpeed ("V Pan Speed", Float ) = 0

        _OpacityTexture ("Opacity Texture", 2D) = "white" {}
        [MaterialToggle] _MaskOn ("Mask On", Float ) = 1
        _MaskUPanSpeed ("Mask U Pan Speed", Float ) = 0
        _MaskVPanSpeed ("Mask V Pan Speed", Float ) = 0
        
        _DissolveTexture ("Dissolve Texture", 2D) = "white" {}
        [MaterialToggle] _DissolveOn ("Dissolve On", Float ) = 1
        _UDissolveSpeed ("U Dissolve Speed", Float ) = 0
        _VDissolveSpeed ("V Dissolve Speed", Float ) = 0

        [MaterialToggle] _DissolvePowerOn ("Dissolve Power On (or use uv)", Float ) = 0
        _DissolvePower ("DissolvePower", Float ) = 0
        _DissolveSmoothness ("DissolveSmoothness", Float ) = 0


        _DistortTexture ("DistortTexture", 2D) = "white" {}
        [MaterialToggle] _DistortOn ("Distort On", Float ) = 1
        [MaterialToggle] _DistortSmoothOn ("Distort Smooth On", Float ) = 1
        _UDistortSpeed ("U Distort Speed", Float ) = 0
        _VDistortSpeed ("V Distort Speed", Float ) = 0
        _DistortPower ("DistortPower", Float ) = 0

        [MaterialToggle] _FresnelOn ("Fresnel On", Float ) = 1
        [MaterialToggle] _FresnelOneMinus ("Fresnel OneMinus", Float ) = 1
        _FresnelPower (" Fresnel Power", Float ) = 0
        _FresnelVisual ("Fresnel Visual", Range(0, 1)) = 0

        _RampTexture ("RampTexture", 2D) = "white" {}

        [MaterialToggle] _SoftParticleOn ("SoftParticle On", Float ) = 1
        _SoftParticlePower ("SoftParticlePower", Float ) = 1

        [MaterialToggle] _RotatorOn ("Rotator On", Float ) = 0
        _Rotator ("Rotator", Float ) = 0

        _VertexOffsetTexture ("VertexOffsetTexture", 2D) = "white" {}
        [MaterialToggle] _VertexOffsetOn ("VertexOffest On", Float ) = 1
        _VertexOffsetPower ("VertexOffsetPower", Vector) = (1,1,1,0)
        _VertexUPanSpeed ("Vertex U Pan Speed", Float ) = 0
        _VertexVPanSpeed ("Vertex V Pan Speed", Float ) = 0
        
    }
    SubShader
    {
        Tags{"IgnoreProjector" = "True" "Queue" = "Transparent" "RenderType" = "Transparent"}
        LOD 100
        Pass
        {
            Name "FORWARD"
            Tags{
                "LightMode" = "UniversalForward"
            }
            Blend One One
            Cull Off
            ZWrite Off
            ZTest Less

            HLSLPROGRAM
            #pragma vertex vert
			#pragma fragment frag
            #pragma multi_compile_fwdbase
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            half _MainTexAlphaOn;
            half _PanUVOn;
            half _MaskOn;
            half _DissolveOn;
            half _DissolvePowerOn;
            half _DistortOn;
            half _DistortSmoothOn;
            half _FresnelOn;
            half _FresnelOneMinus;
            half _SoftParticleOn;
            half _RotatorOn;
            half _VertexOffsetOn;

            float4 _Color;
            float _UPanSpeed;
            float _VPanSpeed;
            
            float _MaskUPanSpeed;
            float _MaskVPanSpeed;

            float _UDissolveSpeed;
            float _VDissolveSpeed;
            float _DissolvePower;
            float _DissolveSmoothness;

            float _UDistortSpeed;
            float _VDistortSpeed;
            float _DistortPower;

            float _FresnelPower;
            float _FresnelVisual;

            float _SoftParticlePower;
            float _Rotator;

            float4 _VertexOffsetPower;
            float _VertexUPanSpeed;
            float _VertexVPanSpeed;

            float4 _MainTex_ST;
            float4 _OpacityTexture_ST;
            float4 _DissolveTexture_ST;
            float4 _DistortTexture_ST;
            float4 _RampTexture_ST;
            float4 _VertexOffsetTexture_ST;


            TEXTURE2D(_MainTex);
            TEXTURE2D(_CameraDepthTexture);
            TEXTURE2D(_OpacityTexture);
            TEXTURE2D(_DissolveTexture);
            TEXTURE2D(_DistortTexture);
            TEXTURE2D(_RampTexture);
            TEXTURE2D(_VertexOffsetTexture);
            SAMPLER(sampler_MainTex);
            SAMPLER(sampler_OpacityTexture);
            SAMPLER(sampler_DissolveTexture);
            SAMPLER(sampler_DistortTexture);
            SAMPLER(sampler_RampTexture);
            SAMPLER(sampler_VertexOffsetTexture);
            SAMPLER(sampler_CameraDepthTexture);

            struct a2v{
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };

            struct v2f{
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float4 projPos : TEXCOORD4;
                float4 vertexColor : COLOR;
            };

            v2f vert (a2v v)
            {
                v2f o = (v2f)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.normalDir = TransformObjectToWorldNormal(v.normal);

                float2 vertUVPanned = float2(
                    _VertexUPanSpeed*_Time.y + o.uv0.r,
                    _VertexVPanSpeed*_Time.y + o.uv0.g
                    );
                float4 vertOffTexSampled = SAMPLE_TEXTURE2D_LOD(
                    _VertexOffsetTexture,
                    sampler_VertexOffsetTexture,
                    TRANSFORM_TEX(vertUVPanned, _VertexOffsetTexture),
                    0.0
                    );
                v.vertex.xyz += lerp(float3(0.0, 0.0, 0.0), 
                    vertOffTexSampled.rgb * _VertexOffsetPower.rgb
                    * v.normal * o.uv1.g,
                    _VertexOffsetOn
                    );
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.projPos = ComputeScreenPos(o.pos);
                o.projPos.z = -o.posWorld.z;
                return o;
            }

            float4 frag(v2f i,float facing : VFACE) : SV_Target
            {
                // Cull Back
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
 
                float sceneZ = max(0,
                LinearEyeDepth 
                    (
                        //Built-in:UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))
                        SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.projPos.xy / i.projPos.w), 
                        _ZBufferParams
                    ) 
                    - _ProjectionParams.g
                    );
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float4 sampledDistortTex = _DistortTexture.Sample(sampler_DistortTexture, TRANSFORM_TEX(
                    float2(_UDistortSpeed*_Time.y + i.uv0.r, _VDistortSpeed*_Time.y + i.uv0.g),
                    _DistortTexture));

                float2 pannedAndDistortedUV = lerp(
                    float2(_UPanSpeed*_Time.y + i.uv0.r, _VPanSpeed*_Time.y + i.uv0.g),
                    i.uv0 + float2(i.uv1.b, i.uv1.a),
                    -_PanUVOn
                    )
                    +lerp(
                        0, lerp(_DistortPower, i.uv1.g, _DistortSmoothOn)*sampledDistortTex, _DistortOn
                    );
                //float rotateAngle = ((_Rotator*3.141592654)/180.0);
                const float DEG2RAD = 0.0174532925; // (3.141592654 / 180.0)
                
                float rotateAngle = _Rotator * DEG2RAD;
                float cosRA = cos(rotateAngle);
                float sinRA = sin(rotateAngle);
                float2 rotatePivot = float2(0.5,0.5);
                float2 rotation = mul(pannedAndDistortedUV - rotatePivot, float2x2(cosRA,-sinRA,sinRA,cosRA)) + rotatePivot;
                float2 rotatedRes = lerp(pannedAndDistortedUV, rotation, _RotatorOn);

                float4 sampledMainTex = _MainTex.Sample(sampler_MainTex,TRANSFORM_TEX(rotatedRes, _MainTex));

                float4 sampledOpacityTex = _OpacityTexture.Sample(sampler_OpacityTexture, TRANSFORM_TEX(
                    float2(_MaskUPanSpeed*_Time.y + i.uv0.r,_MaskVPanSpeed*_Time.y + i.uv0.g), _OpacityTexture
                    ));
                float4 sampledDissolveTex = _DissolveTexture.Sample(sampler_DissolveTexture, TRANSFORM_TEX(
                    float2(_UDissolveSpeed*_Time.y + i.uv0.r, _VDissolveSpeed*_Time.y + i.uv0.g), _DissolveTexture
                    ));
                float dissolveMode = lerp(1.0 - i.uv1.r, _DissolvePower, _DissolvePowerOn);

                float4 sampledRampTex = _RampTexture.Sample(sampler_RampTexture, TRANSFORM_TEX(i.uv0, _RampTexture));
                float fresnelItem = pow(1.0-max(0,dot(i.normalDir, viewDirection)),_FresnelPower);
                float3 emissive = 
                    sampledRampTex.rgb*
                    (
                        (
                            _Color.rgb*sampledMainTex.rgb*
                            (
                                //lerp(sampledMainTex.g, sampledMainTex.a, _MainTexAlphaOn)*
                                lerp(1.0, sampledOpacityTex.r, _MaskOn)
                            )
                            *i.vertexColor.rgb*i.vertexColor.a
                        )
                        *lerp(1.0, saturate((sceneZ-partZ)/_SoftParticlePower), _SoftParticleOn)
                        *lerp(1.0, lerp(1.0-fresnelItem, fresnelItem, _FresnelOneMinus), _FresnelOn)
                        *lerp(1.0, smoothstep(dissolveMode, dissolveMode+_DissolveSmoothness, sampledDissolveTex.r), _DissolveOn)
                    );
                
                return float4(emissive,1);
            }

            ENDHLSL
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma shader_feature _ALPHATEST_ON

            TEXTURE2D(_VertexOffsetTexture);
            SAMPLER(sampler_VertexOffsetTexture);

            float4 _VertexOffsetTexture_ST;

            float4 _VertexOffsetPower;
            float _VertexUPanSpeed;
            float _VertexVPanSpeed;
            half _VertexOffsetOn;

            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
            };
            struct v2f {
                float2 uv0 : TEXCOORD1;
                float4 uv1 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float4 shadowCoord : TEXCOORD0;  // 用于存储阴影相关数据
                float4 pos : SV_POSITION;        // 裁剪空间位置
            };
            v2f vert (a2v v) {
                v2f o = (v2f)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.normalDir = TransformObjectToWorldNormal(v.normal);

                float4 sampledVertexOffTex = SAMPLE_TEXTURE2D_LOD(
                    _VertexOffsetTexture,
                    sampler_VertexOffsetTexture,
                    TRANSFORM_TEX(
                        float2(_VertexUPanSpeed*_Time.y + o.uv0.r, _VertexVPanSpeed*_Time.y + o.uv0.r),
                        _VertexOffsetTexture
                        ),
                    0.0
                    );
                
                v.vertex.xyz += lerp( 0.0, (sampledVertexOffTex.rgb*_VertexOffsetPower.rgb*v.normal*o.uv1.g), _VertexOffsetOn );
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = TransformObjectToHClip( v.vertex );
                
                return o;
            }
            float4 frag(v2f i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                Light light = GetMainLight(TransformWorldToShadowCoord(i.posWorld));
                half shadow = light.shadowAttenuation;
                return half4(shadow,shadow,shadow,1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
