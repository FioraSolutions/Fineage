using System;

/// <summary>
/// Interlude auto-shot client rules shared by the shortcut bar and EditMode tests.
/// Soulshots, spiritshots, blessed spiritshots and beast shots all use etcitem_type
/// <c>soul_shot</c> in the Classic itemgrp (the zone then picks the charge kind).
/// </summary>
public static class AutoShotClientRules
{
    public const string ShotEtcType = "soul_shot";

    public static bool IsShotEtcType(string etcItemType)
    {
        return !string.IsNullOrEmpty(etcItemType)
            && string.Equals(etcItemType, ShotEtcType, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Official Interlude / Mobius: right-click on a shot shortcut sends RequestAutoSoulShot;
    /// left-click and the hotkey use one shot (UseItem). The bar shine follows ExAutoSoulShot,
    /// not the click itself. Inventory slots do not show the shine.
    /// </summary>
    public static bool RightClickTogglesAuto(bool rightButton) => rightButton;
}

/// <summary>
/// Retail <c>l2_skilltime.ToggleEffect</c> frame names. Toggle skills and armed auto-shots
/// both draw this shine; the files are ToggleEffect001..013 under ShortCut/skill_time.
/// </summary>
public static class ToggleEffectFrames
{
    public const string ResourceRoot = "Data/UI/ShortCut/skill_time/ToggleEffect";
    public const int Count = 13;
    public const long FrameMs = 90;

    public static string ResourcePath(int oneBasedIndex) =>
        ResourceRoot + oneBasedIndex.ToString("000");
}
