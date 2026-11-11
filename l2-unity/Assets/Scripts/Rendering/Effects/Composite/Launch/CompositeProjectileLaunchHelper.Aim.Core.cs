using UnityEngine;
using System.Collections.Generic;

public static partial class CompositeProjectileLaunchHelper
{
    private static ProjectileData BuildSettings(
        CompositePrefabPart part,
        GameObject projectileObject,
        Vector3 startPos,
        Transform target)
    {
        CompositeProjectileConfig projectile = GetProjectileConfig(part);
        ProjectileData settings = projectile.settingsOverride != null
            ? new ProjectileData(projectile.settingsOverride)
            : new ProjectileData();

        settings.prefab = projectileObject;
        settings.transform = projectileObject.transform;
        settings.startPosition = startPos;
        settings.targetTransform = target;
        settings.impactType = projectile.impactType;
        return settings;
    }

    private static ProjectileLaunchMode GetLaunchMode(CompositePrefabPart part)
    {
        return GetProjectileConfig(part).launchMode;
    }

    private static bool ShouldHideUntilShoot(CompositePrefabPart part)
    {
        if (part != null && (part.spawnTiming == CompositePartSpawnTiming.OnHitCollider
                             || part.spawnTiming == CompositePartSpawnTiming.OnAnimationShoot))
        {
            // Collider-hit parts appear after hit; on-shoot-spawned parts do not exist yet at cast start.
            return false;
        }

        CompositeProjectileConfig config = GetProjectileConfig(part);
        return config.launchMode == ProjectileLaunchMode.OnAnimationShoot && !config.showBeforeAnimationShoot;
    }

    private static ProjectileImpactType GetImpactType(CompositePrefabPart part)
    {
        return GetProjectileConfig(part).impactType;
    }
}
