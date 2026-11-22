using UnityEngine;

/// <summary>
/// Interlude option.ini / l2.ini clipping semantics wired for the client visibility manager.
/// UI can land later; values persist in PlayerPrefs with original key names.
/// </summary>
public static class InterludeVisibilityOptions
{
    const string KPawnClip = "opt.clip.pawn";
    const string KTerrainClip = "opt.clip.terrain";
    const string KRenderCharacterCount = "opt.video.rendercharactercount";
    const string KPawnClipPreset = "opt.video.pawnclippingrange";
    const string KTerrainClipPreset = "opt.video.terrainclippingrange";
    const string KModelDetail = "opt.video.modeldetail";

    /// <summary>Stock Interlude [ClippingRange] Pawn default from reference option.ini samples.</summary>
    public const float DefaultPawnClip = 3f;

    /// <summary>Stock Interlude [ClippingRange] Terrain default.</summary>
    public const float DefaultTerrainClip = 8f;

    /// <summary>Typical retail option.ini RENDERCHARACTERCOUNT mid preset.</summary>
    public const int DefaultRenderCharacterCount = 50;

    /// <summary>Meters per unit of [ClippingRange] Pawn (maps UE-style slider to world meters).</summary>
    public const float PawnClipMetersPerUnit = 12f;

    /// <summary>Meters per unit of [ClippingRange] Terrain.</summary>
    public const float TerrainClipMetersPerUnit = 18f;

    static readonly float[] PawnClipPresetToUnits = { 2f, 2.5f, 3f, 4f, 5f, 6f };
    static readonly float[] TerrainClipPresetToUnits = { 4f, 5f, 6f, 8f, 10f, 12f };

    public static float PawnClippingUnits
    {
        get
        {
            if (PlayerPrefs.HasKey(KPawnClip))
                return Mathf.Max(0.5f, PlayerPrefs.GetFloat(KPawnClip, DefaultPawnClip));
            int preset = Mathf.Clamp(PlayerPrefs.GetInt(KPawnClipPreset, 2), 0, PawnClipPresetToUnits.Length - 1);
            return PawnClipPresetToUnits[preset];
        }
        set
        {
            PlayerPrefs.SetFloat(KPawnClip, Mathf.Max(0.5f, value));
            PlayerPrefs.Save();
        }
    }

    public static float TerrainClippingUnits
    {
        get
        {
            if (PlayerPrefs.HasKey(KTerrainClip))
                return Mathf.Max(1f, PlayerPrefs.GetFloat(KTerrainClip, DefaultTerrainClip));
            int preset = Mathf.Clamp(PlayerPrefs.GetInt(KTerrainClipPreset, 3), 0, TerrainClipPresetToUnits.Length - 1);
            return TerrainClipPresetToUnits[preset];
        }
        set
        {
            PlayerPrefs.SetFloat(KTerrainClip, Mathf.Max(1f, value));
            PlayerPrefs.Save();
        }
    }

    public static int RenderCharacterCount
    {
        get => Mathf.Clamp(PlayerPrefs.GetInt(KRenderCharacterCount, DefaultRenderCharacterCount), 8, 512);
        set
        {
            PlayerPrefs.SetInt(KRenderCharacterCount, Mathf.Clamp(value, 8, 512));
            PlayerPrefs.Save();
        }
    }

    /// <summary>ModelDetail 0 = high, higher = cheaper (Interlude [Video] ModelDetail).</summary>
    public static int ModelDetail
    {
        get => Mathf.Clamp(PlayerPrefs.GetInt(KModelDetail, 0), 0, 3);
        set
        {
            PlayerPrefs.SetInt(KModelDetail, Mathf.Clamp(value, 0, 3));
            PlayerPrefs.Save();
        }
    }

    public static float PawnDrawDistanceMeters => PawnClippingUnits * PawnClipMetersPerUnit;

    public static float PawnDrawDistanceSq => PawnDrawDistanceMeters * PawnDrawDistanceMeters;

    public static float TerrainDrawDistanceMeters => TerrainClippingUnits * TerrainClipMetersPerUnit;

    public static float NameplateViewDistanceMeters => Mathf.Min(PawnDrawDistanceMeters, 80f);

    public static float WorldHearDistanceMeters => Mathf.Min(PawnDrawDistanceMeters, AudioWorldConstants.HearMaxMeters);

    public static float TierDistanceScaleForModelDetail()
    {
        return ModelDetail switch
        {
            0 => 1f,
            1 => 0.85f,
            2 => 0.7f,
            _ => 0.55f,
        };
    }
}