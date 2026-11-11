using UnityEngine;
using System;
using UnityEngine.ProBuilder;
using System.Collections.Generic;
using VContainer;

public partial class ProjectileManager
{
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
