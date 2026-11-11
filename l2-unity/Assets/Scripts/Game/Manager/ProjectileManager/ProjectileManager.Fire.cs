public partial class ProjectileManager : AbstractProjectile, IProjectileManager
{
    public int LaunchProjectile(GameObject readyProjectile, Vector3 startPos, Transform target, ProjectileData settings = null, float offset = 0.3f)
    {
        if (readyProjectile == null)
        {
            // Debug.LogError("Ready projectile cannot be null!");
            return -1;
        }
        HIT_OFFSET = offset;
        int attackerEntityId = ResolveProjectileAttackerEntityId(readyProjectile);
        ClearParentObject(readyProjectile);

        Vector3 adjustedTarget = VectorWorldUtils.GetCollision(startPos, target);

        float distance = Vector3.Distance(startPos, adjustedTarget);
        bool isArrowStick = settings != null && settings.impactType == ProjectileImpactType.ArrowStick;

        float flightTime;
        // NArrow + skill bolt share ANProjectile accel (dirMul=3000).
        flightTime = ProjectileFlightTimeCalculator.CalculateL2AccelFlightTimeSeconds(distance);
        if (isArrowStick)
        {
            flightTime = ResolveArrowStickFlightTime(settings, flightTime);
        }

        float requiredSpeed = distance / Mathf.Max(flightTime, 0.01f);
        int projectileId = nextId++;

        if (isArrowStick)
        {
            float settingsFly = settings != null ? settings.flytime : -1f;
            float flyIfDist1500 =
                ProjectileFlightTimeCalculator.CalculateL2ArrowFlightTimeIfConstantSpeed(distance);
            // Debug.Log(
                // $"[BOW_ARROW] LAUNCH id={projectileId} dist3d={distance:F3} " +
                // $"accel={speed:F3} avgSpeed={requiredSpeed:F3} flySec={flightTime:F3} " +
                // $"settingsFly={settingsFly:F3} flyIfDist1500={flyIfDist1500:F3} " +
                // $"start={startPos} aim={adjustedTarget} uuAccel=3000 path=(t/T)^2");
        }
        else
        {
            // Debug.Log(
                // $"[MAGIC_PROJ] LAUNCH id={projectileId} dist3d={distance:F3} " +
                // $"accel={speed:F3} avgSpeed={requiredSpeed:F3} flySec={flightTime:F3} " +
                // $"impact={(settings != null ? settings.impactType.ToString() : "null")} " +
                // $"uuAccel=3000 path=(t/T)^2 uu→unity=/52.5");
        }

        ProjectileData projectileData = CreateData(projectileId, distance, readyProjectile, startPos, target,
            adjustedTarget, requiredSpeed, settings, defaultSettings);
        projectileData.attackerEntityId = attackerEntityId;


        projectileData.flytime = flightTime;
        projectileData.lifetime = flightTime;
        if (isArrowStick)
        {
            // The bow notify is already aligned with Interlude. Compensate only
            // Unity's animator-to-projectile handoff by entering the original
            // (t/T)^2 trajectory 80 ms after its start.
            float launchCompensation = Mathf.Min(
                ArrowAnimatorLaunchCompensationSeconds,
                Mathf.Max(0f, flightTime - 0.01f));
            projectileData.startTime -= launchCompensation;
            // Debug.Log(
                // $"[BOW_ARROW] LAUNCH_COMPENSATION id={projectileId} " +
                // $"compSec={launchCompensation:F3} nominalFlySec={flightTime:F3} " +
                // $"visibleFlySec={Mathf.Max(0f, flightTime - launchCompensation):F3}");
        }
        projectileData.lastPosition = Vector3.zero;
        AttachCastTimingSnapshot(projectileData);
        LogReplayMagicShot(projectileData);
        SetPosition(readyProjectile, startPos);
        var rotation = GetRotation(adjustedTarget, startPos);
        SetRotation(readyProjectile, rotation);
 
        activeProjectiles[projectileId] = projectileData;
        return projectileId;
    }

    private static float ResolveArrowStickFlightTime(ProjectileData settings, float fallbackFlightTime)
    {
        if (settings == null)
        {
            return fallbackFlightTime;
        }

        float configured = settings.flytime;
        if (configured > 0.05f && configured < 4.5f)
        {
            return configured;
        }

        return fallbackFlightTime;
    }

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
