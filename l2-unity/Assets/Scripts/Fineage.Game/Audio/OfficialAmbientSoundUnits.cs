/// <summary>
/// Official AmbientSoundObject property units.
/// Hear distance is <see cref="Ue2SoundAudibility.OuterRadiusMeters"/>:
/// authored SoundRadius times 100, then L2 UU to meters.
/// </summary>
public static class OfficialAmbientSoundUnits
{
    /// <summary>Outer cutoff of an authored SoundRadius, in Unity meters.</summary>
    public static float RadiusToUnity(float officialRadiusUu) =>
        Ue2SoundAudibility.OuterRadiusMeters(officialRadiusUu);

    /// <summary>Official SoundPitch is a signed offset in hundredths; 0 means no change.</summary>
    public static float PitchToMultiplier(float officialPitch)
    {
        if (officialPitch == 0f)
            return 1f;
        return officialPitch / 100f + 1f;
    }

    /// <summary>Official SoundVolume is 0..255.</summary>
    public static float VolumeToScale(float officialVolume)
    {
        if (officialVolume <= 0f)
            return 1f;
        return officialVolume / 255f;
    }

    public static bool TryParseType(string value, out AmbientSoundType type)
    {
        type = AmbientSoundType.AST1_Always;
        if (string.IsNullOrEmpty(value))
            return true;
        if (value == "0" || value == "AST1_Always")
        {
            type = AmbientSoundType.AST1_Always;
            return true;
        }
        if (value == "1" || value == "AST1_Day")
        {
            type = AmbientSoundType.AST1_Day;
            return true;
        }
        if (value == "2" || value == "AST1_Night")
        {
            type = AmbientSoundType.AST1_Night;
            return true;
        }
        if (value == "3" || value == "AST1_Water")
        {
            type = AmbientSoundType.AST1_Water;
            return true;
        }
        if (value == "4" || value == "AST1_RangeTime")
        {
            type = AmbientSoundType.AST1_RangeTime;
            return true;
        }
        return System.Enum.TryParse(value, out type);
    }
}
