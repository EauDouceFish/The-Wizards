Shader "MyTestShaders/Alpha Double_Texture" {
    Properties {
        _SoftPower ("SoftPower", Float ) = 1
        [MaterialToggle] _SoftParticle ("SoftParticle", Float ) = 1
        _Textures ("Textures", 2D) = "white" {}
        [MaterialToggle] _TextureAlpha ("TextureAlpha", Float ) = 0
        _Rotator ("Rotator", Float ) = 0
        [HDR]_Color ("Color", Color) = (0.5,0.5,0.5,1)
        [MaterialToggle] _PanUV ("PanUV", Float ) = 0
        _USpeed ("U Speed", Float ) = 0
        _VSpeed ("V Speed", Float ) = 0
        _Mask_Textures ("Mask_Textures", 2D) = "white" {}
        [MaterialToggle] _MaskTex ("Mask Tex", Float ) = 0
        _Mask_R ("Mask_R", Float ) = 0
        _OpacityU_Speed ("OpacityU_Speed", Float ) = 0
        _OpacityV_Speed ("OpacityV_Speed", Float ) = 0
        _RampColor ("RampColor", 2D) = "white" {}
        _Gam_R ("Gam_R", Float ) = 0
        _Dissolve_TEX ("Disslove_TEX", 2D) = "white" {}
        [MaterialToggle] _Dissolve ("Dissolve", Float ) = 1
        _Smoot ("Smoot", Float ) = 0
        [MaterialToggle] _DissOrPower ("DissOrPower", Float ) = 1
        _DissPower ("DissPower", Float ) = 0
        _Disslov_U_speed ("Disslov_U_speed", Float ) = 0
        _Disslov_V_speed ("Disslov_V_speed", Float ) = 0
        _niuqu_Tex ("niuqu_Tex", 2D) = "white" {}
        _NIUQU ("NIUQU", Float ) = 0
        _N_U_Speed ("N_U_Speed", Float ) = 0
        _N_V_Speed ("N_V_Speed", Float ) = 0
        [MaterialToggle] _NiuQUDiss ("NiuQUDiss", Float ) = 0
        [MaterialToggle] _Fresnel_Oneminus ("Fresnel_Oneminus", Float ) = 1
        [MaterialToggle] _FresnelOp ("Fresnel Op", Float ) = 1
        _FresnelPower (" Fresnel  Power", Float ) = 0
        _VertexT ("VertexT", 2D) = "white" {}
        [MaterialToggle] _VerTexOffest ("VerTexOffest", Float ) = 0
        _VertexPower ("VertexPower", Vector) = (1,1,1,0)
        _VPan_U ("VPan_U", Float ) = 0
        _VPan_V ("VPan_V", Float ) = 0
        _MaskTextures ("MaskTextures", 2D) = "white" {}
        _BackFacePower ("BackFacePower", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 100
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="UniversalForward"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZTest Always
            ZWrite Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
         
            TEXTURE2D(_CameraDepthTexture);
            TEXTURE2D(_Textures);
            TEXTURE2D(_Mask_Textures);  
            TEXTURE2D(_niuqu_Tex); 
            TEXTURE2D(_RampColor); 
            TEXTURE2D(_Dissolve_TEX); 
            TEXTURE2D(_VertexT); 
            TEXTURE2D(_MaskTextures); 
            SAMPLER(sampler_CameraDepthTexture); 
            SAMPLER(sampler_Textures);
            SAMPLER(sampler_Mask_Textures);
            SAMPLER(sampler_niuqu_Tex);
            SAMPLER(sampler_RampColor);
            SAMPLER(sampler_Dissolve_TEX);
            SAMPLER(sampler_VertexT);
            SAMPLER(sampler_MaskTextures);

            float4 _Textures_ST;
            float4 _Mask_Textures_ST;
            float4 _niuqu_Tex_ST;
            float4 _RampColor_ST;
            float4 _Dissolve_TEX_ST;
            float4 _VertexT_ST;
            float4 _MaskTextures_ST;
            float4 _Color;


            half _TextureAlpha;
            half _SoftParticle;
            float _USpeed;
            float _VSpeed;
            half _MaskTex;
            float _FresnelPower;
            half _FresnelOp;
            half _Dissolve;
            half _PanUV;
            float _BackFacePower;
            float _NIUQU;
            float _Rotator;
            float _DissPower;
            float _Smoot;
            float _SoftPower;
            half _DissOrPower;
            half _NiuQUDiss;
            float4 _VertexPower;
            float _VPan_U;
            float _VPan_V;
            half _VerTexOffest;
            float _OpacityU_Speed;
            float _OpacityV_Speed;
            float _N_U_Speed;
            float _N_V_Speed;
            half _Fresnel_Oneminus;
            float _Disslov_U_speed;
            float _Disslov_V_speed;
            float _Mask_R;
            float _Gam_R;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD4;
                //UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.normalDir = TransformObjectToWorldNormal(v.normal);
                float4 _VertexT_var =  SAMPLE_TEXTURE2D_LOD(
                    _VertexT,
                    sampler_VertexT,
                    TRANSFORM_TEX(float2(((_VPan_U*_Time.g)+o.uv0.r),(o.uv0.g+(_Time.g*_VPan_V))),_VertexT),
                    0.0
                    );
                v.vertex.xyz += lerp( 0.0, (_VertexT_var.rgb*_VertexPower.rgb*v.normal*o.uv1.g), _VerTexOffest );
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = TransformObjectToHClip( v.vertex.xyz );
                //UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                o.projPos.z = -o.posWorld.z;
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
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

                const float DEG2RAD = 0.0174532925;
                float cosRA = cos(_Gam_R*DEG2RAD);
                float sinRA = sin(_Gam_R*DEG2RAD);
                float2 rotatePiv = float2(0.5,0.5);
                float4 _RampColor_var = _RampColor.Sample(sampler_RampColor,TRANSFORM_TEX(
                     mul(i.uv0-rotatePiv,float2x2( cosRA, -sinRA, sinRA, cosRA))+rotatePiv,
                     _RampColor));
                float cosRA_2 = cos(_Rotator*DEG2RAD);
                float sinRA_2 = sin(_Rotator*DEG2RAD);
                float4 _niuqu_Tex_var = _niuqu_Tex.Sample(sampler_niuqu_Tex,TRANSFORM_TEX(
                     float2(((_N_U_Speed*_Time.g)+i.uv0.r),(i.uv0.g+(_Time.g*_N_V_Speed))),
                    _niuqu_Tex));
                float niuquR = _niuqu_Tex_var.r*_NIUQU;
                float2 pannedAndDistortedTex = (mul((lerp( float2(((_USpeed*_Time.g)+i.uv0.r),(i.uv0.g+(_Time.g*_VSpeed))), (i.uv0+float2(i.uv1.b,i.uv1.a)), _PanUV )+niuquR)-rotatePiv,float2x2( cosRA_2, -sinRA_2, sinRA_2, cosRA_2))+rotatePiv);
                float4 _Textures_var = _Textures.Sample(sampler_Textures,TRANSFORM_TEX(pannedAndDistortedTex, _Textures));
                float3 vertColor = _Color.rgb*_Textures_var.rgb*i.vertexColor.rgb;
                float3 emissive = (_RampColor_var.rgb*lerp((vertColor*_BackFacePower),vertColor,isFrontFace));
                float3 finalColor = emissive;
                float _TextureAlpha_var = lerp( _Textures_var.r, _Textures_var.a, _TextureAlpha );
                float cosMaskR = cos(_Mask_R*DEG2RAD);
                float sinMaskR = sin(_Mask_R*DEG2RAD);
                float2 opacityAndMask = (mul(float2(((_OpacityU_Speed*_Time.g)+i.uv0.r),(i.uv0.g+(_Time.g*_OpacityV_Speed)))-rotatePiv,float2x2( cosMaskR, -sinMaskR, sinMaskR, cosMaskR))+rotatePiv);
                
                float4 _Mask_Textures_var = _Mask_Textures.Sample(sampler_Mask_Textures,TRANSFORM_TEX(opacityAndMask, _Mask_Textures));
                float fresnel = (1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelPower));
                float _DissOrPower_var = lerp( (1.0 - i.uv1.r), _DissPower, _DissOrPower );
                float2 uvDissolved = float2(((_Disslov_U_speed*_Time.g)+i.uv0.r),(i.uv0.g+(_Time.g*_Disslov_V_speed)));
                float2 _NiuQUDiss_var = lerp( uvDissolved, (niuquR+uvDissolved), _NiuQUDiss );
                float4 _Dissolve_TEX_var = _Dissolve_TEX.Sample(sampler_Dissolve_TEX,TRANSFORM_TEX(_NiuQUDiss_var, _Dissolve_TEX));
                float4 _MaskTextures_var = _MaskTextures.Sample(sampler_MaskTextures,TRANSFORM_TEX(i.uv0, _MaskTextures));
                half4 finalRGBA = half4(finalColor,(_Color.a*(lerp( _TextureAlpha_var, (_TextureAlpha_var*_Mask_Textures_var.r), _MaskTex )*i.vertexColor.a*lerp( 1.0, saturate((sceneZ-partZ)/_SoftPower), _SoftParticle )*lerp( 1.0, lerp( fresnel, (1.0 - fresnel), _Fresnel_Oneminus ), _FresnelOp )*lerp( 1.0, smoothstep( _DissOrPower_var, (_DissOrPower_var+_Smoot), _Dissolve_TEX_var.r ), _Dissolve ))*_MaskTextures_var.r));
                //UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
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
            //#pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            //#pragma multi_compile_fog
            TEXTURE2D(_VertexT);
            SAMPLER(sampler_VertexT);
            float4 _VertexT_ST;
            float4 _VertexPower;
            float _VPan_U;
            float _VPan_V;
            half _VerTexOffest;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
            };
            struct VertexOutput {
                float2 uv0 : TEXCOORD1;
                float4 uv1 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float4 pos : SV_POSITION;
                float4 shadowCoord : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.normalDir = TransformObjectToWorld(v.normal);
                float4 _VertexT_var = SAMPLE_TEXTURE2D_LOD(
                    _VertexT,
                    sampler_VertexT,
                    TRANSFORM_TEX(float2(_VPan_U*_Time.g+o.uv0.r,o.uv0.g+_Time.g*_VPan_V),_VertexT),
                    0.0
                );
                v.vertex.xyz += lerp( 0.0, (_VertexT_var.rgb*_VertexPower.rgb*v.normal*o.uv1.g), _VerTexOffest );
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = TransformObjectToHClip( v.vertex.xyz);
                
                
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                Light light = GetMainLight(TransformWorldToShadowCoord(i.posWorld.xyz));
                half shadow = light.shadowAttenuation;
                return half4(shadow,shadow,shadow,1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
