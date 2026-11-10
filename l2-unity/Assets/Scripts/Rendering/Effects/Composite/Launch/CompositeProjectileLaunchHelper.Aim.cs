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

    private static void RestartHiddenVisuals(BaseEffect spawned)
    {
        L2Particle particle = spawned as L2Particle;
        if (particle != null)
        {
            particle.ResetTimer();
        }
    }

    private static CompositeProjectileConfig GetProjectileConfig(CompositePrefabPart part)
    {
        return part != null && part.projectile != null ? part.projectile : new CompositeProjectileConfig();
    }

    public static void SetPartVisualsVisible(Transform root, bool visible)
    {
        SetVisualsVisible(root, visible);
    }

    private static void SetVisualsVisible(Transform root, bool visible)
    {
        if (root == null)
        {
            return;
        }

        ParticleEmitterV2.SetVisible(root, visible);

        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null || IsOwnedByGpuStream(renderer))
            {
                continue;
            }

            renderer.enabled = visible;
        }

        ParticleSystem[] particleSystems = root.GetComponentsInChildren<ParticleSystem>(true);
        for (int i = 0; i < particleSystems.Length; i++)
        {
            ParticleSystem system = particleSystems[i];
            if (visible)
            {
                system.Play(true);
            }
            else
            {
                system.Pause(true);
                system.Clear(true);
            }
        }
    }

    private static bool IsOwnedByGpuStream(Renderer renderer)
    {
        ParticleStreamDriver driver = renderer.GetComponentInParent<ParticleStreamDriver>();
        if (driver != null && driver.IsGpuDraw)
        {
            return true;
        }

        ParticleGroupV2 groupV2 = renderer.GetComponentInParent<ParticleGroupV2>();
        return groupV2 != null && groupV2.IsGpuDraw;
    }
}
