// Fullscreen Jungle03 fill for login Camera25.
// Draws at far clip with ZTest Always so empty pixels never show solid clear.
Shader "Hidden/L2/LoginJungleFill"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _UvScale ("UV Scale", Vector) = (1, 1, 0, 0)
        _UvOffset ("UV Offset", Vector) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Background"
            "RenderType" = "Opaque"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            Name "LoginJungleFill"
            ZWrite Off
            ZTest Always
            Cull Off
            Blend Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _Tint;
            float4 _UvScale;
            float4 _UvOffset;

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
#if UNITY_REVERSED_Z
                output.positionCS.z = UNITY_RAW_FAR_CLIP_VALUE;
#else
                output.positionCS.z = output.positionCS.w;
#endif
                float2 uv = GetFullScreenTriangleTexCoord(input.vertexID);
                output.uv = uv * _UvScale.xy + _UvOffset.xy;
                return output;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                float4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                return float4(c.rgb * _Tint.rgb, 1);
            }
            ENDHLSL
        }
    }
    Fallback Off
}
