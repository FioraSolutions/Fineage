// Guard NPC body. Same actor vertex light as keltir, different pixel combine.
// Engine.Shader on a_guard_MHuman: Diffuse * Color0 * 2, then add
// SpecularityMask.a * Cube.gold.
// Alpha test is off for the guard. Weapon PNGs are the other case: their alpha is
// the coverage mask (255 inside the islands, 0 on the black padding) and the mesh
// UVs of the padding triangles sit outside every island, so the material must clip
// on it. Draw them without the clip and the padding renders as flat black: the
// black band and the loose black diamonds on a dagger.
// Cube.gold is sampled with the view-space reflection vector's XY.
// VS Out writes that unit vector to TEXCOORD2. There is no 0.5 bias.
// Diffuse and the specularity mask share the mesh UV.
Shader "L2/World/ActorEnvSpecular"
{
    Properties
    {
        [MainTexture] _BaseMap ("Diffuse", 2D) = "white" {}
        _SpecMask ("Specularity Mask", 2D) = "black" {}
        _EnvMap ("Env", 2D) = "black" {}
        [MainColor] _BaseColor ("Color", Color) = (1, 1, 1, 1)
        [Toggle(_ALPHATEST_ON)] _AlphaClip ("Alpha Clip", Float) = 0
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.498039216
        // 1: SpecularityMask alpha (weapon PNG). 0: RGB export dropped the mask, so bright texels shine.
        _SpecFromAlpha ("Spec From Alpha", Float) = 1
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 0
        _ActorAlpha ("Actor Alpha", Range(0, 1)) = 1
        [HideInInspector] _SrcBlend ("Src Blend", Float) = 1
        [HideInInspector] _DstBlend ("Dst Blend", Float) = 0
        [HideInInspector] _ZWrite ("ZWrite", Float) = 1
        [HideInInspector] _MainTex ("BaseMap", 2D) = "white" {}
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "L2Modulate2XLighting.hlsl"
    #include "L2ActorPointLight.hlsl"

    TEXTURE2D(_BaseMap);
    SAMPLER(sampler_BaseMap);
    TEXTURE2D(_SpecMask);
    SAMPLER(sampler_SpecMask);
    TEXTURE2D(_EnvMap);
    SAMPLER(sampler_EnvMap);

    float4 _L2ActorAmbientHalf;
    float4 _L2HsvActorLight;
    float _L2ActorDirectionalScale;

    CBUFFER_START(UnityPerMaterial)
        float4 _BaseMap_ST;
        float4 _BaseColor;
        float _Cull;
        float _Cutoff;
        float _ActorAlpha;
        float _SpecFromAlpha;
    CBUFFER_END

    void L2_ActorClip(float alpha)
    {
        // _Cutoff 0 is not alpha-test off: clip(0) still exists in the shader.
        // The keyword removes the instruction, as in L2ActorModulate2X.
#if defined(_ALPHATEST_ON)
        clip(alpha - _Cutoff);
#endif
    }

    float2 L2_ActorUv(float2 uv)
    {
        float4 st = _BaseMap_ST;
        if (abs(st.x) + abs(st.y) < 0.0001)
            st = float4(1, 1, 0, 0);
        return uv * st.xy + st.zw;
    }

    float3 L2_ActorColor0(float3 worldNormal, float3 worldPos)
    {
        float3 n = normalize(worldNormal);
        float3 sunDir = _L2StaticMeshLightDirection.xyz;
        float lenSq = dot(sunDir, sunDir);
        float ndotl = lenSq > 1e-8 ? saturate(-dot(n, sunDir * rsqrt(lenSq))) : 0.0;
        float3 sunColor = _L2HsvActorLight.rgb * _L2ActorDirectionalScale;
        float3 localSum = L2_ActorPointSum(worldPos, n);
        return saturate(_L2ActorAmbientHalf.rgb + sunColor * ndotl + localSum);
    }

    float2 L2_CameraEnvUv(float3 positionVS, float3 worldNormal)
    {
        float3 incident = normalize(positionVS);
        float3 nVS = normalize(TransformWorldToViewDir(worldNormal));
        float3 reflection = reflect(incident, nVS);
        return reflection.xy;
    }

    float4 L2_ActorEnvRgba(float2 uv, float2 envUv, float3 color0)
    {
        float4 diffuseSample = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
        float3 diffuse = diffuseSample.rgb * _BaseColor.rgb;
        float mask = SAMPLE_TEXTURE2D(_SpecMask, sampler_SpecMask, uv).a;
        if (_SpecFromAlpha < 0.5)
        {
            float luma = dot(diffuse, float3(0.30, 0.59, 0.11));
            mask = saturate((luma - 0.42) * 1.8);
        }
        float3 env = SAMPLE_TEXTURE2D(_EnvMap, sampler_EnvMap, envUv).rgb;
        float3 base = L2_Modulate2X(diffuse, color0);
        // Coverage comes from the diffuse alpha, not the specularity mask: the weapon
        // alpha is both, but the padding is 0 either way and the clip needs it.
        return float4(saturate(base + mask * env), diffuseSample.a * _BaseColor.a);
    }
    ENDHLSL

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "UniversalMaterialType" = "Unlit"
            "IgnoreProjector" = "True"
        }

        Cull [_Cull]
        ZWrite [_ZWrite]
        ZTest LEqual
        Blend [_SrcBlend] [_DstBlend]

        Pass
        {
            Name "L2ActorEnvForwardOnly"
            Tags { "LightMode" = "UniversalForwardOnly" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 2.0
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma multi_compile_instancing

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 color0 : COLOR;
                float viewAbsZ : TEXCOORD1;
                float3 positionVS : TEXCOORD2;
                float3 normalWS : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = posInputs.positionCS;
                OUT.uv = L2_ActorUv(IN.uv);
                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.color0 = L2_ActorColor0(normalWS, posInputs.positionWS);
                OUT.viewAbsZ = abs(posInputs.positionVS.z);
                OUT.positionVS = posInputs.positionVS;
                OUT.normalWS = normalWS;
                return OUT;
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                float2 envUv = L2_CameraEnvUv(IN.positionVS, IN.normalWS);
                float4 rgba = L2_ActorEnvRgba(IN.uv, envUv, IN.color0);
                L2_ActorClip(rgba.a);
                float3 rgb = L2_LinearFog(rgba.rgb, IN.viewAbsZ);
                return half4(rgb, saturate(rgba.a * _ActorAlpha));
            }
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            ZWrite On
            ColorMask R
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex DepthVert
            #pragma fragment DepthFrag
            #pragma target 2.0
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma multi_compile_instancing

            struct DepthAttrs
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct DepthVaryings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            DepthVaryings DepthVert(DepthAttrs IN)
            {
                DepthVaryings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                OUT.positionCS = GetVertexPositionInputs(IN.positionOS.xyz).positionCS;
                OUT.uv = L2_ActorUv(IN.uv);
                return OUT;
            }

            half DepthFrag(DepthVaryings IN) : SV_Target
            {
                float alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv).a * _BaseColor.a;
                L2_ActorClip(alpha);
                return 0;
            }
            ENDHLSL
        }

        Pass
        {
            Name "DepthNormalsOnly"
            Tags { "LightMode" = "DepthNormalsOnly" }
            ZWrite On
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex DepthNormalsVert
            #pragma fragment DepthNormalsFrag
            #pragma target 2.0
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma multi_compile_instancing

            struct DNAttrs
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct DNVaryings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float2 uv : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            DNVaryings DepthNormalsVert(DNAttrs IN)
            {
                DNVaryings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                OUT.positionCS = GetVertexPositionInputs(IN.positionOS.xyz).positionCS;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = L2_ActorUv(IN.uv);
                return OUT;
            }

            half4 DepthNormalsFrag(DNVaryings IN) : SV_Target
            {
                float alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv).a * _BaseColor.a;
                L2_ActorClip(alpha);
                float3 n = normalize(IN.normalWS);
                return half4(n * 0.5 + 0.5, 0);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
