Shader "Unlit/Jade"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _BacklightPower ("Backlight Power", Float) = 5.0
        _BacklightIntensity ("Backlight Intensity", Range(0, 10)) = 1.0
        _FrontlightPower ("Frontlight Power", Float) = 5.0
        _FrontlightIntensity ("Frontlight Intensity", Range(0, 10)) = 1.0
        _DistortPower ("DistortPower", Range(0, 1)) = 0.5
        _Shiness("Shiness", Float) = 64
        _ThicknessMap("Thickness Map", 2D) = "white" {}
        _RefCubeMap("RefCubeMap", Cube) = "white" {}

        [Toggle(_AdditionalLights)] _AddLights ("AddLights", Float) = 1
    }
    SubShader
    {
        Tags { "LightMode" = "UniversalForward" "RenderType"="Opaque" }
        LOD 100
        Cull Off
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma shader_feature _AdditionalLights

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS 
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE  
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS 
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS 
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct a2v
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float3 normal :NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 posWorld : TEXCOORD1;
                float3 normalWorld: TEXCOORD2;
            };

            TEXTURECUBE(_RefCubeMap);
            TEXTURE2D(_MainTex);
            TEXTURE2D(_ThicknessMap);
            SAMPLER(sampler_RefCubeMap);
            SAMPLER(sampler_MainTex);
            SAMPLER(sampler_ThicknessMap);
            float4 _RefCubeMap_HDR;
            float4 _MainTex_ST;
            float4 _ThicknessMap_ST;
            float4 _Color;
            float _BacklightPower;
            float _BacklightIntensity;
            float _FrontlightPower;
            float _FrontlightIntensity;
            float _DistortPower;
            float _Shiness;

            v2f vert (a2v v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                o.normalWorld = TransformObjectToWorldNormal(v.normal);
                o.posWorld = TransformObjectToWorld(v.vertex.xyz);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
              
                // 计算光照先计算环境光
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half3 albedo = texColor.rgb;
                //half3 ambient = _GlossyEnvironmentColor.rgb * albedo;
                
                // 计算主光源漫反射：获取法线视线方向，计算漫反射+计算背光透射内部折射
                float3 mainLightDir = normalize(_MainLightPosition.xyz);
                float3 normalDir = normalize(i.normalWorld);
                //float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.posWorld);
                float3 viewDir = normalize(TransformWorldToViewDir(i.posWorld));
                    // 计算表面漫反射:
                float nDotL =  max(0.0, dot(normalDir, mainLightDir));
                float3 frontlightDir = normalize(mainLightDir + normalDir * _DistortPower);
                float3 backlightDir = -frontlightDir;
                float vDotB = max(0.0, dot(viewDir, backlightDir));
                float vDotF = max(0.0, dot(viewDir, frontlightDir));
                float backlight = max(0.0, pow(vDotB, _BacklightPower)) * _BacklightIntensity ;
                float frontlight = max(0.0, pow(vDotF, _FrontlightPower)) * _FrontlightIntensity;


                float3 diffuse = (frontlight + backlight)* albedo * _MainLightColor.xyz;
                
                // 计算主光源高光
                half3 worldViewDir = normalize(TransformWorldToViewDir(i.posWorld));
                half3 halfDirWorld = normalize(mainLightDir + worldViewDir);
                //half3 halfDirWorld = normalize(mainLightDir + viewDir);
                half spec = pow(max(0.0, dot(normalDir, halfDirWorld)), _Shiness);
                half smoothWFactor = fwidth(spec) * 2.0;
                half3 specular =  spec * smoothWFactor * _MainLightColor.rgb;

                float thickness = 1.0 - SAMPLE_TEXTURE2D(_ThicknessMap, sampler_ThicknessMap, i.uv).r;

                float3 reflectionDir = reflect(-viewDir, normalDir);
                float4 refColorHDR = SAMPLE_TEXTURECUBE(_RefCubeMap, sampler_RefCubeMap, reflectionDir);

                float fresnel = 1.0 - max(0.0, dot(normalDir, viewDir));
                float3 refEnvColor = DecodeHDREnvironment(refColorHDR, _RefCubeMap_HDR);
                refEnvColor *= fresnel;

                Light light = GetMainLight(TransformWorldToShadowCoord(i.posWorld));
                float3 finalColor = (specular + diffuse)*thickness *light.shadowAttenuation;

                  // 计算多光源阴影
                #if defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
				    half4 shadowMask = inputData.shadowMask;
				#elif !defined (LIGHTMAP_ON)
				    half4 shadowMask = unity_ProbesOcclusion;
				#else
				    half4 shadowMask = half4(1, 1, 1, 1);
				#endif

                // 处理多光源
                #ifdef _AdditionalLights
                    half3 additionalLights = half3(0, 0, 0); // 初始化为零
                    int addLightsCount = GetAdditionalLightsCount(); // 获取额外光源的数量

                    for (int j = 0; j < addLightsCount; j++)
                    {
                        // 获取第 j 个附加光源
                        Light addLight = GetAdditionalLight(j,i.posWorld, shadowMask);
                        float3 addLightDir = normalize(addLight.direction); // 附加光源的方向
                        //float3 addLightHalfDir = normalize(addLightDir + worldViewDir); // 附加光源的半向量

                        // 计算副光源漫反射:
                        float3 addFrontlightDir = normalize(addLightDir + normalDir * _DistortPower);
                        float3 addBacklightDir = -addFrontlightDir;
                        float addVDotB = max(0.0, dot(viewDir, addBacklightDir));
                        float addVDotF = max(0.0, dot(viewDir, addFrontlightDir));
                        float addBacklight = max(0.0, pow(addVDotB, _BacklightPower)) * _BacklightIntensity;
                        float addFrontlight = max(0.0, pow(addVDotF, _FrontlightPower)) * _FrontlightIntensity;
                        float3 addDiffuse = (addBacklight + addFrontlight) * addLight.color.rgb;

                        // 计算副光源高光
                        half3 addHalfDirWorld = normalize(addLightDir + worldViewDir);
                        half addSpec = pow(max(0.0, dot(normalDir, addHalfDirWorld)), _Shiness);
                        half addSmoothWFactor = fwidth(addSpec) * 2.0;
                        half3 addSpecular =  spec * addSmoothWFactor * addLight.color.rgb;

                        float3 addFinal = (addDiffuse + addSpecular)* albedo ;

                        // 衰减处理 (根据距离进行衰减)
                        float attenuation = addLight.distanceAttenuation;
                        float addLightShadowAttenuation = addLight.shadowAttenuation;
                        // 计算附加光源的贡献
                        additionalLights += addFinal * attenuation * addLightShadowAttenuation;
                    }
                    finalColor += additionalLights;
                #endif
                finalColor *= _Color.xyz;
                //finalColor += refEnvColor* 0.05;
                return half4(finalColor, 1.0);
                //return half4(refEnvColor, 1.0);
            }
            ENDHLSL
        }
        UsePass "Universal Render Pipeline/Lit/ShadowCaster" 
    }
}
