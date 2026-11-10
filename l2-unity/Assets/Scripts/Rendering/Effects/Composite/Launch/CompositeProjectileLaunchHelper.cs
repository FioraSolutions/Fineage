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

    /// <summary>
    /// True if this composite must listen for the caster animation shoot event (projectile on shoot and/or parts spawned on shoot).
    /// </summary>
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

    public static bool ShouldLaunchOnShoot(CompositePrefabPart part)
    {
        return GetLaunchMode(part) == ProjectileLaunchMode.OnAnimationShoot;
    }

    public static bool ShouldLaunchImmediately(CompositePrefabPart part)
    {
        return GetLaunchMode(part) == ProjectileLaunchMode.Immediate;
    }

    public static bool IsProjectilePart(CompositePrefabPart part)
    {
        ProjectileLaunchMode mode = GetLaunchMode(part);
        return mode == ProjectileLaunchMode.Immediate || mode == ProjectileLaunchMode.OnAnimationShoot;
    }

    public static bool TryLaunch(
        CompositePrefabPart part,
        BaseEffect spawned,
        Transform targetTransform,
        float playStartedAt,
        string debugPrefix)
    {
        if (part == null || spawned == null || targetTransform == null || ProjectilePacketServices.Projectiles == null)
        {
            return false;
        }

        Transform projectileTransform = spawned.transform;
        projectileTransform.SetParent(null, true);
        Vector3 startPos = projectileTransform.position;

        ProjectileData settings = BuildSettings(part, spawned.gameObject, startPos, targetTransform);
        ProjectilePacketServices.Projectiles.LaunchProjectile(
            spawned.gameObject,
            startPos,
            targetTransform,
            settings);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        float now = Time.time;
        float elapsed = playStartedAt > 0f ? now - playStartedAt : -1f;
        // Debug.Log(
            // $"{debugPrefix} LaunchPartAsProjectile part='{part.name}' at={now:F3}s elapsed={elapsed:F3}s " +
            // $"start={startPos} target='{targetTransform.name}' impactType={GetImpactType(part)}.");
#endif
        return true;
    }

    public static void ProcessShootLaunches(
        CompositePrefabPart[] parts,
        Dictionary<CompositePrefabPart, BaseEffect> spawnedPartInstances,
        HashSet<CompositePrefabPart> launchedProjectileParts,
        Transform targetTransform,
        float playStartedAt,
        string debugPrefix)
    {
        if (parts == null || spawnedPartInstances == null || launchedProjectileParts == null || targetTransform == null)
        {
            return;
        }

        for (int i = 0; i < parts.Length; i++)
        {
            CompositePrefabPart part = parts[i];
            if (part == null || !ShouldLaunchOnShoot(part))
            {
                continue;
            }

            if (launchedProjectileParts.Contains(part))
            {
                continue;
            }

            if (!spawnedPartInstances.TryGetValue(part, out BaseEffect spawned) || spawned == null)
            {
                continue;
            }

            if (TryLaunch(part, spawned, targetTransform, playStartedAt, debugPrefix))
            {
                launchedProjectileParts.Add(part);
            }
        }
    }

}
