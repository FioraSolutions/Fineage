using System;
using UnityEngine;

public partial class HitManager
{
    public bool TryPrepareProjectileEffectHit(
        GameObject projectilePrefab,
        Vector3 hitPoint,
        Vector3 hitDirection,
        int attackerEntityId,
        Func<Transform, bool> isFromTrackedProjectile,
        out Vector3 resolvedHitPoint,
        out Vector3 resolvedHitDirection)
    {
        resolvedHitPoint = hitPoint;
        resolvedHitDirection = Vector3.forward;

        if (projectilePrefab == null || isFromTrackedProjectile == null)
        {
            return false;
        }

        Transform attacker = projectilePrefab.transform;
        if (attacker == null || !isFromTrackedProjectile(attacker))
        {
            return false;
        }

        Entity attackerEntity = ResolveEntityFromWorld(attackerEntityId);
        Entity targetEntity = null;
        if (attackerEntity != null && attackerEntity.TargetId > 0)
        {
            targetEntity = ResolveEntityFromWorld(attackerEntity.TargetId);
        }

        resolvedHitDirection = ResolveSoulshotImpactDirection(attackerEntity, targetEntity, resolvedHitPoint);
        if (resolvedHitDirection.sqrMagnitude < 0.0001f && hitDirection.sqrMagnitude > 0.0001f)
        {
            Vector3 flat = hitDirection;
            flat.y = 0f;
            resolvedHitDirection = flat.sqrMagnitude > 0.0001f ? flat.normalized : Vector3.forward;
        }

        Vector3 spawnPoint = ResolveSoulshotSpawnPoint(targetEntity, resolvedHitDirection, resolvedHitPoint);
        EffectManager effects = GlobalEffectsPacketServices.Effects;
        if (effects == null)
            return false;

        if (attackerEntity != null && attackerEntity.IsSoulshotCharged)
        {
            effects.PlayerImpactEffect(SoulshotImpactEffectId, spawnPoint, resolvedHitDirection);
            attackerEntity.IsSoulshotCharged = false;
            return true;
        }

        if (attackerEntity != null && attackerEntity.IsSpiritshotCharged)
            attackerEntity.IsSpiritshotCharged = false;

        effects.PlayerImpactEffect(NormalImpactEffectId, spawnPoint, resolvedHitDirection);
        return true;
    }

    private static void LogSoulshotImpactOrientation(
        Entity attacker,
        Entity target,
        Vector3 hitPoint,
        Vector3 spawnPoint,
        Vector3 resolvedImpactDir)
    {
        Transform attackerTf = ResolveAttackerTransform(attacker);
        Transform targetTf = target != null ? target.transform : null;

        Vector3 attackerPos = attackerTf != null ? attackerTf.position : Vector3.zero;
        Vector3 attackerFwd = Flat(attackerTf != null ? attackerTf.forward : Vector3.forward);
        Vector3 targetPos = targetTf != null ? targetTf.position : hitPoint;
        Vector3 targetFwd = Flat(targetTf != null ? targetTf.forward : Vector3.forward);
        float radius = ResolveTargetCollisionRadius(target);

        Vector3 playerToHit = Flat(hitPoint - attackerPos);
        Vector3 playerToTarget = Flat(targetPos - attackerPos);
        Vector3 resolved = Flat(resolvedImpactDir);

        float angResolvedVsP2T = AngleDeg(resolved, playerToTarget);
        float angResolvedVsP2H = AngleDeg(resolved, playerToHit);
        float angResolvedVsAttackerFwd = AngleDeg(resolved, attackerFwd);
        float angResolvedVsTargetFwd = AngleDeg(resolved, targetFwd);

        string source = attackerTf != null && targetTf != null ? "PLAYER_TO_TARGET_ENTITY" : "PLAYER_TO_HIT_FALLBACK";
        int attackerId = attacker != null && attacker.Identity != null ? attacker.Identity.Id : 0;
        int targetId = target != null && target.Identity != null ? target.Identity.Id : 0;

        // Debug.Log(
            // $"{IMPACT_DEBUG_TAG} soulshotOrient source={source} " +
            // $"attackerId={attackerId} targetId={targetId} " +
            // $"attackerPos={attackerPos} attackerFwd={attackerFwd} " +
            // $"targetPos={targetPos} targetFwd={targetFwd} hitPoint={hitPoint} " +
            // $"spawnPoint={spawnPoint} collisionRadius={radius:F3} " +
            // $"playerToTarget={playerToTarget} playerToHit={playerToHit} resolved={resolved} " +
            // $"angResolvedVsP2T={angResolvedVsP2T:F1} angResolvedVsP2H={angResolvedVsP2H:F1} " +
            // $"angResolvedVsAttackerFwd={angResolvedVsAttackerFwd:F1} angResolvedVsTargetFwd={angResolvedVsTargetFwd:F1}");
    }
}
