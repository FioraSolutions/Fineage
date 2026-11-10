using UnityEngine;

/// <summary>
/// Interlude nameplate colours: white default, purple while PvP-flagged, red while karma &gt; 0.
/// Karma wins over flag (Mobius CharInfo name colour).
/// </summary>
public static class NameplateColors
{
    public static readonly Color PurpleFlag = new Color(0.78f, 0.45f, 1f, 1f);
    public static readonly Color PkRed = new Color(1f, 0.25f, 0.25f, 1f);

    public static Color Resolve(Color defaultColor, int karma, long pvpFlag)
    {
        if (karma > 0)
        {
            float t = Mathf.Clamp01(karma / 1000f);
            return Color.Lerp(Color.white, PkRed, Mathf.Max(0.55f, t));
        }
        if (pvpFlag > 0)
            return PurpleFlag;
        return defaultColor;
    }

    public static Color Resolve(Entity entity, Color defaultColor)
    {
        if (entity == null)
            return defaultColor;
        int karma = 0;
        long flag = 0;
        if (entity.Stats is PlayerStats ps)
            karma = ps.Karma;
        if (entity.Status is PlayerStatus status)
            flag = status.PvpFlag;
        return Resolve(defaultColor, karma, flag);
    }
}
