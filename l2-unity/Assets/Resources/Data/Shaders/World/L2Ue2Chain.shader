// One UE2 world material path. Slots and ops come from L2Ue2MaterialChain:
// Shader diffuse / opacity / specular / self-illumination / detail,
// Combiner, TexEnvMap, and per-slot UV (TexPanner, TexOscillator, TexScaler).
Shader "L2/World/Ue2Chain"
{
    Properties
    {
        [MainTexture] _BaseMap ("Diffuse", 2D) = "white" {}
        _OpacityMap ("Opacity", 2D) = "white" {}
        _SpecularMap ("Specular", 2D) = "black" {}
        _SpecMask ("Specularity Mask", 2D) = "white" {}
        _EmissionMap ("Self Illumination", 2D) = "black" {}
        _EmissionMask ("Self Illumination Mask", 2D) = "white" {}
        _DetailMap ("Detail", 2D) = "white" {}
        _EnvMap ("Env", 2D) = "black" {}
        _CombineMap ("Combiner B", 2D) = "white" {}
        _CombineMask ("Combiner Mask", 2D) = "white" {}
        [MainColor] _BaseColor ("Color", Color) = (1, 1, 1, 1)
        _DetailScale ("Detail Scale", Float) = 1
        _CombineOp ("Combine Op", Float) = -1
        _AlphaOp ("Alpha Op", Float) = -1
        _EnvMapType ("Env Type", Float) = -1
        _HasOpacity ("Has Opacity", Float) = 0
        _HasSpec ("Has Specular", Float) = 0
        _HasSpecMask ("Has Spec Mask", Float) = 0
        _HasEmit ("Has Emission", Float) = 0
        _HasEmitMask ("Has Emission Mask", Float) = 0
        _HasDetail ("Has Detail", Float) = 0
        _HasEnv ("Has Env", Float) = 0
        _HasCombine ("Has Combine", Float) = 0
        _HasCombineMask ("Has Combine Mask", Float) = 0
        _Modulate2X ("Modulate 2X", Float) = 1
        _ModulateSpecular2X ("Modulate Specular 2X", Float) = 0
        _LightSpecular ("Light Specular", Float) = 0
        _AlphaClip ("Alpha Clip", Float) = 0
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
        _ZWrite ("ZWrite", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst", Float) = 0
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "L2Modulate2XLighting.hlsl"

    TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
    TEXTURE2D(_OpacityMap); SAMPLER(sampler_OpacityMap);
    TEXTURE2D(_SpecularMap); SAMPLER(sampler_SpecularMap);
    TEXTURE2D(_SpecMask); SAMPLER(sampler_SpecMask);
    TEXTURE2D(_EmissionMap); SAMPLER(sampler_EmissionMap);
    TEXTURE2D(_EmissionMask); SAMPLER(sampler_EmissionMask);
    TEXTURE2D(_DetailMap); SAMPLER(sampler_DetailMap);
    TEXTURE2D(_EnvMap); SAMPLER(sampler_EnvMap);
    TEXTURE2D(_CombineMap); SAMPLER(sampler_CombineMap);
    TEXTURE2D(_CombineMask); SAMPLER(sampler_CombineMask);

    CBUFFER_START(UnityPerMaterial)
        float4 _BaseMap_ST;
        float4 _OpacityMap_ST;
        float4 _SpecularMap_ST;
        float4 _SpecMask_ST;
        float4 _EmissionMap_ST;
        float4 _EmissionMask_ST;
        float4 _DetailMap_ST;
        float4 _EnvMap_ST;
        float4 _CombineMap_ST;
        float4 _CombineMask_ST;
        float4 _BaseColor;
        float _DetailScale;
        float _CombineOp;
        float _AlphaOp;
        float _EnvMapType;
        float _HasOpacity;
        float _HasSpec;
        float _HasSpecMask;
        float _HasEmit;
        float _HasEmitMask;
        float _HasDetail;
        float _HasEnv;
        float _HasCombine;
        float _HasCombineMask;
        float _Modulate2X;
        float _ModulateSpecular2X;
        float _LightSpecular;
        float _AlphaClip;
        float _Cutoff;
        float _Cull;
        float _ZWrite;
        float _SrcBlend;
        float _DstBlend;
    CBUFFER_END

    float4 L2_Combine(float4 a, float4 b, float4 mask)
    {
        float4 c = a;
        if (_CombineOp < 0.5) c = a;
        else if (_CombineOp < 1.5) c = b;
        else if (_CombineOp < 2.5) c = a * b;
        else if (_CombineOp < 3.5) c = a + b;
        else if (_CombineOp < 4.5) c = a - b;
        else if (_CombineOp < 5.5) c = lerp(a, b, mask.a);
        else if (_CombineOp < 6.5) c = a + b * mask.a;
        else c.rgb = mask.rgb;

        float al = c.a;
        if (_AlphaOp < -0.5) al = c.a;
        else if (_AlphaOp < 0.5) al = mask.a;
        else if (_AlphaOp < 1.5) al = a.a * b.a;
        else if (_AlphaOp < 2.5) al = saturate(a.a + b.a);
        else if (_AlphaOp < 3.5) al = a.a;
        else al = b.a;
        c.a = al;
        return c;
    }

    float2 L2_EnvUv(float3 normalWS, float3 positionWS)
    {
        float3 n = normalize(normalWS);
        float3 view = normalize(GetCameraPositionWS() - positionWS);
        float3 r = reflect(-view, n);
        float2 uv;
        if (_EnvMapType < 0.5)
            uv = r.xy * 0.5 + 0.5;
        else if (_EnvMapType < 1.5)
        {
            float3 rvs = mul((float3x3)UNITY_MATRIX_V, r);
            uv = rvs.xy;
        }
        else
            uv = r.xy * 0.5 + 0.5;
        return uv * _EnvMap_ST.xy + _EnvMap_ST.zw;
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
            Name "L2Ue2ChainForward"
            Tags { "LightMode" = "UniversalForwardOnly" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0
            #pragma multi_compile_instancing

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float4 color : COLOR;
                float viewAbsZ : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                VertexPositionInputs pos = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = pos.positionCS;
                OUT.positionWS = pos.positionWS;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.uv;
                float3 vtx = IN.color.rgb;
                if (max(max(vtx.r, vtx.g), vtx.b) < 0.001)
                    vtx = float3(1, 1, 1);
                OUT.color = float4(vtx, IN.color.a);
                OUT.viewAbsZ = abs(pos.positionVS.z);
                return OUT;
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                float2 uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                float4 diff = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv) * _BaseColor;
                if (_HasCombine > 0.5)
                {
                    float4 b = SAMPLE_TEXTURE2D(_CombineMap, sampler_CombineMap, TRANSFORM_TEX(IN.uv, _CombineMap));
                    float4 mask = _HasCombineMask > 0.5
                        ? SAMPLE_TEXTURE2D(_CombineMask, sampler_CombineMask, TRANSFORM_TEX(IN.uv, _CombineMask))
                        : float4(1, 1, 1, 1);
                    diff = L2_Combine(diff, b, mask);
                }
                if (_HasDetail > 0.5)
                {
                    // TRANSFORM_TEX appends .xy. A scalar max() in the first argument
                    // is an invalid Metal subscript. Scale first, then transform.
                    float2 detailUv = IN.uv * max(_DetailScale, 0.001);
                    float2 duv = TRANSFORM_TEX(detailUv, _DetailMap);
                    diff.rgb *= SAMPLE_TEXTURE2D(_DetailMap, sampler_DetailMap, duv).rgb;
                }

                float alpha = diff.a;
                if (_HasOpacity > 0.5)
                    alpha = SAMPLE_TEXTURE2D(_OpacityMap, sampler_OpacityMap, TRANSFORM_TEX(IN.uv, _OpacityMap)).a;
                // D3DCMP_GREATER: AlphaRef 0 still rejects alpha 0. clip(alpha - 0) does not.
                if (_AlphaClip > 0.5 && alpha <= _Cutoff)
                    clip(-1);

                float3 ambient = L2_OrWhite(_L2StaticMeshAmbient.rgb);
                float3 color0 = saturate(IN.color.rgb * ambient * 0.5);
                float3 rgb = _Modulate2X > 0.5 ? L2_Modulate2X(diff.rgb, color0) : saturate(diff.rgb);

                if (_HasEmit > 0.5)
                {
                    float3 emit = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, TRANSFORM_TEX(IN.uv, _EmissionMap)).rgb;
                    if (_HasEmitMask > 0.5)
                        emit *= SAMPLE_TEXTURE2D(_EmissionMask, sampler_EmissionMask, TRANSFORM_TEX(IN.uv, _EmissionMask)).a;
                    rgb += emit;
                }

                if (_HasSpec > 0.5 || _HasEnv > 0.5)
                {
                    float3 spec = 0;
                    if (_HasEnv > 0.5)
                        spec = SAMPLE_TEXTURE2D(_EnvMap, sampler_EnvMap, L2_EnvUv(IN.normalWS, IN.positionWS)).rgb;
                    else
                        spec = SAMPLE_TEXTURE2D(_SpecularMap, sampler_SpecularMap, TRANSFORM_TEX(IN.uv, _SpecularMap)).rgb;
                    if (_HasSpecMask > 0.5)
                        spec *= SAMPLE_TEXTURE2D(_SpecMask, sampler_SpecMask, TRANSFORM_TEX(IN.uv, _SpecMask)).a;
                    if (_LightSpecular > 0.5)
                        spec *= color0;
                    if (_ModulateSpecular2X > 0.5)
                        spec *= 2.0;
                    rgb += spec;
                }

                return half4(rgb, alpha);
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
            #pragma target 3.0
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
                OUT.uv = IN.uv;
                return OUT;
            }

            half DepthFrag(DepthVaryings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                float alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, TRANSFORM_TEX(IN.uv, _BaseMap)).a;
                if (_HasOpacity > 0.5)
                    alpha = SAMPLE_TEXTURE2D(_OpacityMap, sampler_OpacityMap, TRANSFORM_TEX(IN.uv, _OpacityMap)).a;
                if (_AlphaClip > 0.5 && alpha <= _Cutoff)
                    clip(-1);
                return 0;
            }
            ENDHLSL
        }
    }
    FallBack Off
}
