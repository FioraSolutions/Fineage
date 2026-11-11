using UnityEngine;
using System;
using UnityEngine.ProBuilder;
using System.Collections.Generic;
using VContainer;

public partial class ProjectileManager
{
    private int ResolveProjectileAttackerEntityId(GameObject projectilePrefab)
    {
        if (projectilePrefab == null)
        {
            return 0;
        }

        Entity ownerEntity = projectilePrefab.GetComponentInParent<Entity>();
        if (ownerEntity != null && ownerEntity.Identity != null)
        {
            return ownerEntity.Identity.Id;
        }

        if (DomainPacketServices.LocalPlayer != null && DomainPacketServices.LocalPlayer.Identity != null)
        {
            return DomainPacketServices.LocalPlayer.Identity.Id;
        }

        return 0;
    }

    private void RefreshHitPositionFromAnchor(ProjectileData projectile)
    {
        if (projectile == null)
        {
            return;
        }

        Transform fallbackTarget = projectile.targetTransform;
        Entity targetEntity = fallbackTarget != null ? fallbackTarget.GetComponentInParent<Entity>() : null;
        Transform anchor = HitAnchorResolver.ResolveHitAnchor(targetEntity, fallbackTarget);
        Vector3 hitPoint = anchor != null ? anchor.position : projectile.targetPosition;

        projectile.hitPoint = hitPoint;
        projectile.hitNormal = Vector3.up;
        projectile.hitDirection = VectorUtils.CalcHitDirection(hitPoint, projectile.startPosition);
    }

    private void LogProjectileImpactTiming(ProjectileData projectile)
    {
        LogReplayMagicHit(projectile);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        float projectileElapsed = Time.time - projectile.startTime;
        if (projectile.castStartTimeSnapshot <= 0f)
        {
        // Debug.Log(
            // $"{MagicProjectileSyncTag} IMPACT id={projectile.id} impactType={projectile.impactType} " +
            // $"globalSinceCastStart=-1 castSnapshot=null flytime={projectile.flytime:F3}s elapsed={projectileElapsed:F3}s.");
            return;
        }

        float globalSinceCastStart = Time.time - projectile.castStartTimeSnapshot;
        // Debug.Log(
            // $"{MagicProjectileSyncTag} IMPACT id={projectile.id} impactType={projectile.impactType} " +
            // $"launchGlobalFromCast={projectile.projectileLaunchGlobalFromCast:F3}s " +
            // $"globalSinceCastStart={globalSinceCastStart:F3}s serverShoot={projectile.castServerShootSnapshot:F3}s serverHit={projectile.castServerHitSnapshot:F3}s " +
            // $"deltaLaunchToShoot={projectile.projectileLaunchGlobalFromCast - projectile.castServerShootSnapshot:F3}s " +
            // $"deltaToServerHit={globalSinceCastStart - projectile.castServerHitSnapshot:F3}s " +
            // $"configuredFly={projectile.flytime:F3}s elapsedSinceLaunch={projectileElapsed:F3}s.");
#endif
    }
}
