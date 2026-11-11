using UnityEngine;
using System.Collections.Generic;

public static partial class CompositeProjectileLaunchHelper
{
    public static void ApplyPreShootVisibility(CompositePrefabPart part, BaseEffect spawned, string debugPrefix)
    {
        if (part == null || spawned == null || !ShouldHideUntilShoot(part))
        {
            return;
        }

        SetVisualsVisible(spawned.transform, false);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Debug.Log($"{debugPrefix} HidePartUntilShoot part='{part.name}' launchMode=OnAnimationShoot.");
#endif
    }

    public static void RevealOnShootParts(
        CompositePrefabPart[] parts,
        Dictionary<CompositePrefabPart, BaseEffect> spawnedPartInstances)
    {
        if (parts == null || spawnedPartInstances == null)
        {
            return;
        }

        for (int i = 0; i < parts.Length; i++)
        {
            CompositePrefabPart part = parts[i];
            if (part == null || !ShouldHideUntilShoot(part))
            {
                continue;
            }

            if (!spawnedPartInstances.TryGetValue(part, out BaseEffect spawned) || spawned == null)
            {
                continue;
            }

            RestartHiddenVisuals(spawned);
            SetVisualsVisible(spawned.transform, true);
        }
    }

    public static bool RequiresAnimationShootLaunch(CompositePrefabPart[] parts)
    {
        if (parts == null)
        {
            return false;
        }

        for (int i = 0; i < parts.Length; i++)
        {
            if (ShouldLaunchOnShoot(parts[i]))
            {
                return true;
            }
        }

        return false;
    }

    public static bool RequiresAnimationShootEvent(CompositePrefabPart[] parts)
    {
        if (parts == null)
        {
            return false;
        }

        for (int i = 0; i < parts.Length; i++)
        {
            CompositePrefabPart part = parts[i];
            if (part == null)
            {
                continue;
            }

            if (ShouldLaunchOnShoot(part))
            {
                return true;
            }

            if (part.homeProjectile != null && part.homeProjectile.ShouldLaunchOnShoot)
            {
                return true;
            }

            if (part.spawnTiming == CompositePartSpawnTiming.OnAnimationShoot)
            {
                return true;
            }
        }

        return false;
    }
}
