using System;
using UnityEngine;

public static partial class L2StaticMeshVertexLight
{
    public const float UuToMeters = 1f / 52.5f;

    /// <summary>
    /// ALight::WorldLightRadius reads the saved LightRadius and returns (radius + 1) * 25.
    /// </summary>

    public struct Light
    {
        public int Type;
        public Vector3 Position;
        public Vector3 Direction;
        public float Radius;
        public Color Color;
        public int Cone;
        public string Name;
    }
}
