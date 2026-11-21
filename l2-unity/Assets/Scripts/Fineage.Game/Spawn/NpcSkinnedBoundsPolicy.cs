using UnityEngine;

public static class NpcSkinnedBoundsPolicy
{
    /// <summary>
    /// <see cref="Npcgrp.CollisionHeight"/> is stored in meters after npcgrp table load (already / 52.5).
    /// </summary>
    public static float ExpectedBodyHeightMeters(Entity entity, float defaultMeters = 1.8f)
    {
        if (entity == null || entity.Appearance == null)
            return defaultMeters;
        return Mathf.Max(0.5f, entity.Appearance.CollisionHeight);
    }

    public static float MaxSkinnedBoundsHeightMeters(float bodyHeightMeters) =>
        bodyHeightMeters * 2.75f + 0.4f;
}