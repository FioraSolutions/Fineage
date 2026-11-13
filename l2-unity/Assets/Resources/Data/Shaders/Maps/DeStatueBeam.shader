// Dark Elf village statue: the vertical streak on O_De_02 (De_Statue004).
// The source texture is a dim crack on black. Lit-transparent swallows it.
// Unlit additive, scrolling up the beam.
Shader "L2/Maps/DeStatueBeam"
{
    Properties
    {
        _BaseMap ("Beam", 2D) = "white" {}
        _BaseColor ("Color", Color) = (0.65, 0.88, 1, 1)
        _Boost ("Boost", Float) = 10
        _Scroll ("Scroll XY", Vector) = (0, 1.4, 0, 0)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Cull Off
        ZWrite Off
        Blend One One

        Pass
        {
            Name "DeStatueBeam"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float _Boost;
                float4 _Scroll;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                o.uv = input.uv * _BaseMap_ST.xy + _BaseMap_ST.zw + _Scroll.xy * _Time.y;
                return o;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                half glow = max(tex.a, max(tex.r, max(tex.g, tex.b)));
                half3 rgb = max(tex.rgb, glow.xxx) * _BaseColor.rgb * _Boost;
                return half4(rgb, 1);
            }
            ENDHLSL
        }
    }
}
