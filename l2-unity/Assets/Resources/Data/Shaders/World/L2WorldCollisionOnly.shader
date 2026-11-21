Shader "L2/World/CollisionOnly"
{
    // UE2 collision / anti-portal slots: participate in mesh collider but write no color.
    Properties { }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-1" }
        Pass
        {
            ColorMask 0
            ZWrite On
            Cull Back
        }
    }
    Fallback Off
}