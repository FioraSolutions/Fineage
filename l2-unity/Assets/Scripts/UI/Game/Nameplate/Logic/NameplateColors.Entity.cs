using UnityEngine;

/// <summary>ACS Entity bridge for <see cref="NameplateColors"/>.</summary>
public static class NameplateColorsEntity
{
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
        return NameplateColors.Resolve(defaultColor, karma, flag);
    }
}
