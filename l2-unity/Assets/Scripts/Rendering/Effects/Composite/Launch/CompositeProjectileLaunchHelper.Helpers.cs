using UnityEngine;
using System.Collections.Generic;

public static partial class CompositeProjectileLaunchHelper
{
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
