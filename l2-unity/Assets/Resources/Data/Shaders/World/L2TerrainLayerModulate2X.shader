// L2 terrain overlay (grass / sand / scorched / …). Same Color0 as the base:
//   rgb = saturate(t0 * Color0 * 2)
//   a   = alphamap (t1 .r)
//   fog on rgb, then Blend SrcAlpha / InvSrcAlpha.
// Color0 is NOT lerp-then-light: light first, then alpha over the opaque dirt.
// Drawn by L2TerrainOverlayRenderPass (CommandBuffer.DrawMesh, AfterRenderingOpaques).
// URP Deferred ignores UniversalForwardOnly in Draw Transparent Objects.
Shader "L2/World/TerrainLayerModulate2X"
{
    Properties
    {
        _LayerTex ("Layer (t0 color)", 2D) = "white" {}
        [NoScaleOffset] _LayerMask ("Layer mask (t1, this layer AlphaMap)", 2D) = "black" {}
        [NoScaleOffset] _Layers ("Layer array (slice = _SplatSlice)", 2DArray) = "white" {}
        [NoScaleOffset] _Splatmaps ("Splat array (slice = _SplatSlice)", 2DArray) = "black" {}
        _UseSplatArrays ("Use splat arrays", Float) = 0
        _LayerIndex ("Layer Index (official overlay, RenderDoc)", Float) = 1
        _SplatSlice ("Splat slice (layerIndex - 1)", Float) = 0
        _LayerUV ("Layer repeat UU (Scale * UScale)", Vector) = (0, 0, 0, 0)
        _LayerXform ("Layer rotation/axis", Vector) = (0, 0, 0, 0)
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "L2Modulate2XLighting.hlsl"
    #include "L2TerrainCommon.hlsl"

    TEXTURE2D(_LayerTex);
    SAMPLER(sampler_LayerTex);
    TEXTURE2D(_LayerMask);
    SAMPLER(sampler_LayerMask);
    TEXTURE2D_ARRAY(_Layers);
    SAMPLER(sampler_Layers);
    TEXTURE2D_ARRAY(_Splatmaps);

    CBUFFER_START(UnityPerMaterial)
        float4 _LayerTex_ST;
        float4 _LayerUV;
        float4 _LayerXform;
        float _LayerIndex;
        float _SplatSlice;
        float _UseSplatArrays;
        float _Cull;
    CBUFFER_END
    ENDHLSL

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent-500"
            "UniversalMaterialType" = "Unlit"
            "IgnoreProjector" = "True"
        }

        Cull [_Cull]
        ZWrite Off
        ZTest LEqual
        Offset -1, -1
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGBA

        Pass
        {
            Name "L2TerrainLayerForwardOnly"
            Tags { "LightMode" = "UniversalForwardOnly" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 4.5
            #pragma multi_compile_instancing

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 worldUu : TEXCOORD0;
                float3 color0 : TEXCOORD1;
                float viewAbsZ : TEXCOORD2;
                float2 meshUv : TEXCOORD3;
                float heightUu : TEXCOORD4;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = posInputs.positionCS;
                OUT.worldUu = posInputs.positionWS.xz * 52.5;
                OUT.heightUu = posInputs.positionWS.y * 52.5;
                float3 vtx = IN.color.rgb;
                if (max(max(vtx.r, vtx.g), vtx.b) < 0.001)
                    vtx = float3(1, 1, 1);
                float intensity = L2_TerrainIntensityAt(OUT.worldUu);
                OUT.color0 = L2_TerrainColor0(vtx, _L2TerrainAmbient.rgb, intensity);
                OUT.meshUv = IN.uv;
                OUT.viewAbsZ = abs(posInputs.positionVS.z);
                return OUT;
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                if (_L2TerrainLightDebug > 0.5)
                    return half4(L2_TerrainLightDebugColor(IN.worldUu), 1);
                float2 colorUv = L2_TerrainLayerUv(IN.worldUu, IN.heightUu, _LayerUV, _LayerXform.x, _LayerXform.y);
                float2 maskUv = L2_TerrainSplatUv(IN.meshUv);
                half3 albedo;
                half alpha;
                if (_UseSplatArrays > 0.5)
                {
                    albedo = SAMPLE_TEXTURE2D_ARRAY(_Layers, sampler_Layers, colorUv, _SplatSlice).rgb;
                    alpha = SAMPLE_TEXTURE2D_ARRAY_BIAS(_Splatmaps, sampler_LinearClamp, maskUv, _SplatSlice, -0.5).r;
                }
                else
                {
                    albedo = SAMPLE_TEXTURE2D(_LayerTex, sampler_LayerTex, colorUv).rgb;
                    alpha = SAMPLE_TEXTURE2D_BIAS(_LayerMask, sampler_LinearClamp, maskUv, -0.5).r;
                }
                float3 rgb = L2_LinearFog(L2_Modulate2X(albedo, IN.color0), IN.viewAbsZ);
                return half4(rgb, alpha);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
