public enum EmitterState
{
    Idle = 0,
    Emitting = 1,
    Draining = 2,
    Complete = 3
}

public enum EmitterStopMode
{
    Drain = 0,
    Kill = 1
}

/// <summary>
/// Runtime form of UE2 EParticleCoordinateSystem. Relative is zero so older
/// prefabs and callers which do not author this field retain existing behavior.
/// </summary>
public enum L2ParticleCoordinateSystem
{
    Relative = 0,
    Independent = 1,
    Spray = 2,
    Absolute = 3,
    RelativeRotation = 4,
    RelativePosition = 5,
    ScreenAbsolute = 6,
    ScreenRelative = 7
}

public static class L2ParticleCoordinateSystemUtil
{
    public const int NativeIndependent = 0;
    public const int NativeRelative = 1;
    public const int NativeAbsolute = 2;
    public const int NativeRelativeRotation = 3;
    public const int NativeSpray = 4;
    public const int NativeRelativePosition = 5;
    public const int NativeScreenAbsolute = 6;
    public const int NativeScreenRelative = 7;

    public static int ParseNative(string value)
    {
        string coordinateSystem = string.IsNullOrEmpty(value)
            ? "1"
            : value.Trim();

        if (int.TryParse(coordinateSystem, out int nativeValue) &&
            nativeValue >= NativeIndependent &&
            nativeValue <= NativeScreenRelative)
        {
            return nativeValue;
        }

        if (coordinateSystem.IndexOf("Independent", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return NativeIndependent;
        if (coordinateSystem.IndexOf("RelativeRotation", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return NativeRelativeRotation;
        if (coordinateSystem.IndexOf("RelativePosition", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return NativeRelativePosition;
        if (coordinateSystem.IndexOf("ScreenAbsolute", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return NativeScreenAbsolute;
        if (coordinateSystem.IndexOf("ScreenRelative", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return NativeScreenRelative;
        if (coordinateSystem.IndexOf("Spray", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return NativeSpray;
        if (coordinateSystem.IndexOf("Absolute", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return NativeAbsolute;
        return NativeRelative;
    }

    public static L2ParticleCoordinateSystem FromNative(int nativeValue)
    {
        switch (nativeValue)
        {
            case NativeIndependent: return L2ParticleCoordinateSystem.Independent;
            case NativeAbsolute: return L2ParticleCoordinateSystem.Absolute;
            case NativeRelativeRotation: return L2ParticleCoordinateSystem.RelativeRotation;
            case NativeSpray: return L2ParticleCoordinateSystem.Spray;
            case NativeRelativePosition: return L2ParticleCoordinateSystem.RelativePosition;
            case NativeScreenAbsolute: return L2ParticleCoordinateSystem.ScreenAbsolute;
            case NativeScreenRelative: return L2ParticleCoordinateSystem.ScreenRelative;
            default: return L2ParticleCoordinateSystem.Relative;
        }
    }

    public static int ToNative(L2ParticleCoordinateSystem coordinateSystem)
    {
        switch (coordinateSystem)
        {
            case L2ParticleCoordinateSystem.Independent: return NativeIndependent;
            case L2ParticleCoordinateSystem.Absolute: return NativeAbsolute;
            case L2ParticleCoordinateSystem.RelativeRotation: return NativeRelativeRotation;
            case L2ParticleCoordinateSystem.Spray: return NativeSpray;
            case L2ParticleCoordinateSystem.RelativePosition: return NativeRelativePosition;
            case L2ParticleCoordinateSystem.ScreenAbsolute: return NativeScreenAbsolute;
            case L2ParticleCoordinateSystem.ScreenRelative: return NativeScreenRelative;
            default: return NativeRelative;
        }
    }

    public static L2ParticleCoordinateSystem Parse(string value)
    {
        return FromNative(ParseNative(value));
    }

    public static bool FreezesSpawnMatrix(L2ParticleCoordinateSystem coordinateSystem)
    {
        return coordinateSystem == L2ParticleCoordinateSystem.Independent ||
               coordinateSystem == L2ParticleCoordinateSystem.Spray;
    }
}

public enum ParticleLifetimePolicy
{
    Authored = 0,
    EmissionWindowFromCast = 1,
    /// <summary>
    /// L2 AdjustparticleLife: stretch LifetimeRange + FadeOutStart to the cast window.
    /// Used when authoring.stretchParticleLifeToWindow (UC MaxParticles=1, no respawn).
    /// </summary>
    StretchParticleLifetimeToCast = 2
}
