using System;

/// <summary>
/// Interlude enchant-glow rules: placement from weapongrp, color/intensity from a
/// level table. No per-item special cases.
/// </summary>
public static partial class EnchantGlow
{
    public const int MaxLevel = 127;
    public const float UuToMeters = 0.01f;

    /// <summary>Interlude weapongrp_interlude.txt column indices (0-based).</summary>
    public const int InterludeRangeA = 70;
    public const int InterludeRangeB = 71;
    public const int InterludeScaleAX = 72;
    public const int InterludeOffsetAX = 75;
    public const int InterludeScaleBX = 78;
    public const int InterludeOffsetBX = 81;

    public const string KindWeapon = "weapon";
    public const string KindArmor = "armor";
    public const string DefaultMesh = "LineageWeapons.rangesample";

    public static int Cap(int level)
    {
        if (level < 0)
            return 0;
        if (level > MaxLevel)
            return MaxLevel;
        return level;
    }

    public static EnchantGlowPlacement PickHand(EnchantGlowPlacement right, EnchantGlowPlacement left, bool leftHand)
    {
        if (leftHand && left.Present)
            return left;
        return right;
    }
}

[Serializable]
public struct EnchantGlowPlacement
{
    public bool Present;
    public string Mesh;
    public float ScaleX;
    public float ScaleY;
    public float ScaleZ;
    public float OffsetX;
    public float OffsetY;
    public float OffsetZ;

    public static EnchantGlowPlacement None => default;

    public static EnchantGlowPlacement From(string mesh, float sx, float sy, float sz, float ox, float oy, float oz)
    {
        var placement = new EnchantGlowPlacement
        {
            Mesh = mesh ?? "",
            ScaleX = sx,
            ScaleY = sy,
            ScaleZ = sz,
            OffsetX = ox,
            OffsetY = oy,
            OffsetZ = oz
        };
        placement.Present = IsMesh(placement.Mesh);
        return placement;
    }

    public static bool IsMesh(string mesh)
    {
        if (string.IsNullOrWhiteSpace(mesh))
            return false;
        if (string.Equals(mesh, "None", StringComparison.OrdinalIgnoreCase))
            return false;
        if (mesh == "0" || mesh == "[None]" || mesh == "[0]")
            return false;
        return true;
    }
}

public readonly struct EnchantGlowStyle
{
    public readonly int Level;
    public readonly float R;
    public readonly float G;
    public readonly float B;
    public readonly float Intensity;

    public EnchantGlowStyle(int level, float r, float g, float b, float intensity)
    {
        Level = level;
        R = r;
        G = g;
        B = b;
        Intensity = intensity;
    }
}
