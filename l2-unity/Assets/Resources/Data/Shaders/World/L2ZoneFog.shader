// One distance-fog path for every world material, including URP Lit.
// Samples the camera color and depth after the world is drawn and applies
// the same L2_LinearFog formula the zone pushes into _L2Fog*.
Shader "Hidden/L2/ZoneFog"
{
    // Blit.hlsl already defines the fullscreen triangle vertex type and Vert.
    // A second definition in each pass is a Metal redefinition (the two passes
    // are one translation unit). Both passes use that type; texcoord is its UV.
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
    #include "L2Modulate2XLighting.hlsl"

    TEXTURE2D(_L2ZoneFogSource);
    SAMPLER(sampler_L2ZoneFogSource);
    TEXTURE2D_X(_CameraDepthTexture);
    SAMPLER(sampler_CameraDepthTexture);

    // Uploaded on the command buffer before the procedural draw. That draw does
    // not inherit Unity's per-camera _ZBufferParams; left at 0, every real
    // depth becomes infinite and the pass fogs the frame.
    float4 _L2ZBufferParams;
    float _L2ReversedZ;
    float _L2FogFarClip;
    ENDHLSL

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

            float4 FragCopy(Varyings i) : SV_Target
            {
                return SAMPLE_TEXTURE2D(_L2ZoneFogSource, sampler_L2ZoneFogSource, i.texcoord);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Fog"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment FragFog

            float4 FragFog(Varyings i) : SV_Target
            {
                float4 src = SAMPLE_TEXTURE2D(_L2ZoneFogSource, sampler_L2ZoneFogSource, i.texcoord);
                float raw = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord).r;
                // Cleared depth is 0 on reversed-Z (Metal) and 1 otherwise. A missing
                // depth texture reads that clear and used to fog every pixel.
                bool farPlane = _L2ReversedZ > 0.5 ? (raw <= 1e-6) : (raw >= 1.0 - 1e-6);
                float eye = LinearEyeDepth(raw, _L2ZBufferParams);
                bool usable = !farPlane && eye > 0.0 && eye < _L2FogFarClip * 0.999;
                float3 rgb = src.rgb;
                if (usable)
                    rgb = L2_LinearFog(rgb, eye);
                return float4(rgb, src.a);
            }
            ENDHLSL
        }
    }
}
