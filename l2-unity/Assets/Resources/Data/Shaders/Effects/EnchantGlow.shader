// Additive unlit mesh for Interlude weapon enchant glow (rangesample stand-in).
Shader "L2/Effects/EnchantGlow"
{
    Properties
    {
        [HDR] _BaseColor ("Color", Color) = (0.35, 0.55, 1, 0.6)
        _Intensity ("Intensity", Float) = 1
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
        ZTest LEqual
        Blend SrcAlpha One

        Pass
        {
            Name "EnchantGlow"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _Intensity;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float intensity = max(_Intensity, 0);
                float4 color = _BaseColor;
                color.rgb *= intensity;
                color.a *= saturate(intensity);
                return color;
            }
            ENDHLSL
        }
    }
    FallBack Off
}
