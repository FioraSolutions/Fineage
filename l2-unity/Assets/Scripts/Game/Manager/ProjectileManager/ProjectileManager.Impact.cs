using UnityEngine;

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

    static void LogReplayMagicShot(ProjectileData projectile)
    {
        if (projectile == null || projectile.impactType == ProjectileImpactType.ArrowStick)
            return;
        Entity caster = ResolveReplayEntity(projectile.attackerEntityId);
        Entity target = projectile.targetTransform != null
            ? projectile.targetTransform.GetComponentInParent<Entity>()
            : null;
        if (caster == null || caster.Identity == null)
        {
            // Debug.Log("[MAGIC_SHOT_LOG] SKIP session shot attackerId=" + projectile.attackerEntityId +
                // " casterNull=" + (caster == null));
            return;
        }
        // Debug.Log("[MAGIC_SHOT_LOG] SESSION shot nick=" + caster.Identity.Name +
            // " id=" + caster.Identity.Id +
            // " attackerId=" + projectile.attackerEntityId);
        // UnityReplaySession.OnMagicShot(
            // caster,
            // target,
            // VectorUtils.ConvertPosUnityToL2j(projectile.startPosition));
    }

    static void LogReplayMagicHit(ProjectileData projectile)
    {
        if (projectile == null || projectile.impactType == ProjectileImpactType.ArrowStick)
            return;
        Entity caster = ResolveReplayEntity(projectile.attackerEntityId);
        Entity target = projectile.targetTransform != null
            ? projectile.targetTransform.GetComponentInParent<Entity>()
            : null;
        Vector3 hit = projectile.hitPoint;
        if (hit == Vector3.zero)
            hit = projectile.targetPosition;
        if (caster == null || caster.Identity == null)
        {
            // Debug.Log("[MAGIC_SHOT_LOG] SKIP session hit attackerId=" + projectile.attackerEntityId +
                // " casterNull=" + (caster == null));
            return;
        }
        // Debug.Log("[MAGIC_SHOT_LOG] SESSION hit nick=" + caster.Identity.Name +
            // " id=" + caster.Identity.Id);
        // UnityReplaySession.OnMagicHit(
            // caster,
            // target,
            // VectorUtils.ConvertPosUnityToL2j(hit));
    }

    static Entity ResolveReplayEntity(int objectId)
    {
        if (objectId <= 0)
            return null;
        if (DomainPacketServices.LocalPlayer != null && DomainPacketServices.LocalPlayer.Identity != null &&
            DomainPacketServices.LocalPlayer.Identity.Id == objectId)
            return DomainPacketServices.LocalPlayer;
        World world = GamePacketServices.GameWorld;
        return world != null ? world.GetEntityNoLockSync(objectId) : null;
    }
}
