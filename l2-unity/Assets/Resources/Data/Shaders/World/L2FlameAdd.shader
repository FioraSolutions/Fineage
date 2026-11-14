// Unlit additive flame card. Black texels add nothing, so the 24-bit
// Flameset frames (de_fire_0000..0015) read as a fire without an alpha mask.
Shader "L2/World/FlameAdd"
{
    Properties
    {
        [MainTexture] _BaseMap ("Texture", 2D) = "white" {}
        [MainColor] _BaseColor ("Color", Color) = (1, 1, 1, 1)
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
            Name "FlameAdd"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float3 FlameObjectScale()
            {
                return float3(
                    length(float3(UNITY_MATRIX_M._m00, UNITY_MATRIX_M._m10, UNITY_MATRIX_M._m20)),
                    length(float3(UNITY_MATRIX_M._m01, UNITY_MATRIX_M._m11, UNITY_MATRIX_M._m21)),
                    length(float3(UNITY_MATRIX_M._m02, UNITY_MATRIX_M._m12, UNITY_MATRIX_M._m22)));
            }

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                // Flameset is one camera-facing card. The imported mesh is a cross,
                // so the second card is edge-on and reads as a peeled white strip.
                float3 pivot = TransformObjectToWorld(float3(0.0, 0.0, 0.0));
                float3 scale = FlameObjectScale();
                float3 toCamera = _WorldSpaceCameraPos - pivot;
                toCamera.y = 0.0;
                float3 right = float3(1.0, 0.0, 0.0);
                float toCameraLenSq = dot(toCamera, toCamera);
                if (toCameraLenSq > 1e-8)
                    right = normalize(cross(float3(0.0, 1.0, 0.0), toCamera * rsqrt(toCameraLenSq)));

                float3 nWS = TransformObjectToWorldNormal(IN.normalOS);
                float nLen = dot(nWS, nWS);
                float align = 1.0;
                if (nLen > 1e-8 && toCameraLenSq > 1e-8)
                    align = abs(dot(nWS * rsqrt(nLen), toCamera * rsqrt(toCameraLenSq)));

                float3 worldPos = pivot;
                // The card whose normal faces the camera stays. Width may be local X or Z.
                // The crossed card collapses, so it is not a bright edge beside the flame.
                if (align >= 0.35)
                {
                    float side = IN.positionOS.x * scale.x;
                    float depth = IN.positionOS.z * scale.z;
                    if (abs(depth) > abs(side))
                        side = depth;
                    worldPos = pivot
                        + right * side
                        + float3(0.0, 1.0, 0.0) * (IN.positionOS.y * scale.y);
                }

                OUT.positionCS = TransformWorldToHClip(worldPos);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                half3 rgb = tex.rgb * _BaseColor.rgb;
                // 32-bit frames store the shape in alpha. 24-bit frames do not.
                half mask = tex.a < 0.99h ? tex.a : 1.0h;
                return half4(rgb * mask, 1);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
