// One distance-fog path for every world material, including URP Lit.
// Samples the camera color and depth after the world is drawn and applies
// the same L2_LinearFog formula the zone pushes into _L2Fog*.
Shader "Hidden/L2/ZoneFog"
{
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            Name "Copy"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment FragCopy
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            TEXTURE2D(_L2ZoneFogSource);
            SAMPLER(sampler_L2ZoneFogSource);

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(uint id : SV_VertexID)
            {
                Varyings o;
                o.positionCS = GetFullScreenTriangleVertexPosition(id);
                o.uv = GetFullScreenTriangleTexCoord(id);
                return o;
            }

            float4 FragCopy(Varyings i) : SV_Target
            {
                return SAMPLE_TEXTURE2D(_L2ZoneFogSource, sampler_L2ZoneFogSource, i.uv);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Fog"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment FragFog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "L2Modulate2XLighting.hlsl"

            TEXTURE2D(_L2ZoneFogSource);
            SAMPLER(sampler_L2ZoneFogSource);
            TEXTURE2D_X(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(uint id : SV_VertexID)
            {
                Varyings o;
                o.positionCS = GetFullScreenTriangleVertexPosition(id);
                o.uv = GetFullScreenTriangleTexCoord(id);
                return o;
            }

            float4 FragFog(Varyings i) : SV_Target
            {
                float4 src = SAMPLE_TEXTURE2D(_L2ZoneFogSource, sampler_L2ZoneFogSource, i.uv);
                float raw = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv).r;
                // Sky cards force the far plane. Fogging that depth replaces the
                // backdrop with the fog color. World pixels have a real depth.
                #if defined(UNITY_REVERSED_Z)
                bool farPlane = raw <= 1e-6;
                #else
                bool farPlane = raw >= 1.0 - 1e-6;
                #endif
                float3 rgb = src.rgb;
                if (!farPlane)
                    rgb = L2_LinearFog(rgb, LinearEyeDepth(raw, _ZBufferParams));
                return float4(rgb, src.a);
            }
            ENDHLSL
        }
    }
}
