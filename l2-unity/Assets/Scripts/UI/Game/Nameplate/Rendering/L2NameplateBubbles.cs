using UnityEngine;

public static partial class L2NameplateBubbles
{
    public const float BubbleWidth = 19f;
    public const float BubbleHeight = 16f;
    public const float BubbleYOffset = 1f; // nameY - 1

    // HeadDisplay_DF_Target_* opaque bbox (all three textures).
    public const float AtlasSize = 32f;
    public const float ContentU = 18f;
    public const float ContentV = 19f;

    public const string NormalResourcePath = "Data/UI/Target/HeadDisplay_DF_Target_Normal";
    public const string TargetResourcePath = "Data/UI/Target/HeadDisplay_DF_Target_Target";
    public const string AttackResourcePath = "Data/UI/Target/HeadDisplay_DF_Target_Attack";

    /// <summary>
    /// Append left+right HeadDisplay spheres for one nameplate line.
    /// Canvas top-down pixels; <paramref name="nameLeft"/> / <paramref name="nameW"/> match name glyphs.
    /// </summary>

}
