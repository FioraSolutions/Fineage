using System;
using UnityEngine.ProBuilder;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class ProjectileManager : AbstractProjectile, IProjectileManager
{
    private void AttachCastTimingSnapshot(ProjectileData projectile)
    {
        MagicCastData castData = ResolveLaunchCastData(projectile);
        if (castData == null)
        {
            projectile.castStartTimeSnapshot = -1f;
            projectile.castServerShootSnapshot = -1f;
            projectile.castServerHitSnapshot = -1f;
            projectile.projectileLaunchGlobalFromCast = -1f;
            return;
        }

        projectile.castStartTimeSnapshot = castData.StartTime;
        projectile.castServerShootSnapshot = castData.serverTimeToShoot;
        projectile.castServerHitSnapshot = castData.HitTime;
        projectile.projectileLaunchGlobalFromCast = Time.time - castData.StartTime;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Debug.Log(
            // $"{MagicProjectileSyncTag} LAUNCH id={projectile.id} impactType={projectile.impactType} " +
            // $"launchGlobalFromCast={projectile.projectileLaunchGlobalFromCast:F3}s " +
            // $"serverShoot={projectile.castServerShootSnapshot:F3}s serverHit={projectile.castServerHitSnapshot:F3}s " +
            // $"deltaLaunchToShoot={projectile.projectileLaunchGlobalFromCast - projectile.castServerShootSnapshot:F3}s " +
            // $"configuredFly={projectile.flytime:F3}s distance={projectile.distance:F3}m.");
#endif
    }

    static MagicCastData ResolveLaunchCastData(ProjectileData projectile)
    {
        if (projectile != null && projectile.attackerEntityId != 0)
        {
            Entity caster = EntityActionSkill.ResolveEntity(projectile.attackerEntityId);
            if (caster != null)
            {
                MagicCastData fromCaster = caster.GetMagicCastData();
                if (fromCaster != null)
                    return fromCaster;
            }
        }

        return DomainPacketServices.LocalPlayer != null ? DomainPacketServices.LocalPlayer.GetMagicCastData() : null;
    }

    public void StopProjectile(int projectileId)
    {
        if (activeProjectiles.TryGetValue(projectileId, out ProjectileData projectile))
        {
            projectile.isActive = false;
            if (projectile.prefab != null && projectile.impactType == ProjectileImpactType.EffectOnly)
            {
                Destroy(projectile.prefab);
            }
            activeProjectiles.Remove(projectileId);
        }
    }
}
