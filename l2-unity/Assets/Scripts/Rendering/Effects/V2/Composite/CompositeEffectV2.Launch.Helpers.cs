using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public partial class CompositeEffectV2 : CompositePrefabEffect
{
    void TryLaunchOnShoot(CompositePart part, BaseEffect instance)
    {
        if (part == null || instance == null || _launchedProjectiles.Contains(part))
        {
            return;
        }

        HomeFlightPart home = part as HomeFlightPart;
        if (home != null)
        {
            if (home.ShouldLaunchOnShoot && home.TryLaunch(instance, _context, HomeProjectiles))
            {
                _launchedProjectiles.Add(part);
            }

            return;
        }

        ShotProjectilePart shot = part as ShotProjectilePart;
        if (shot == null || !shot.ShouldLaunchOnShoot)
        {
            return;
        }

        if (shot.TryLaunch(instance, _context))
        {
            _launchedProjectiles.Add(part);
        }
    }

    bool ShouldLaunchShotOnSpawn(CompositePart part)
    {
        ShotProjectilePart shot = part as ShotProjectilePart;
        if (shot == null || !shot.ShouldLaunchOnShoot)
        {
            return false;
        }

        if (_castData != null && _castData.SpawnShotVisualsImmediately)
        {
            return true;
        }

        if (_castData == null)
        {
            return true;
        }

        float shootAt = _castData.StartTime + Mathf.Max(0f, _castData.serverTimeToShoot);
        return Time.time + 0.05f >= shootAt;
    }

    void ExtendLifetimeThroughProjectileImpact(EffectSettings lifeTimeSettings)
    {
        if (lifeTimeSettings == null ||
            _castData == null ||
            _context == null ||
            _context.TargetTransform == null ||
            _v2Parts == null)
        {
            return;
        }

        float requiredLifetime = lifeTimeSettings.defaultLifeTime;
        for (int i = 0; i < _v2Parts.Length; i++)
        {
            ShotProjectilePart shot = _v2Parts[i] as ShotProjectilePart;
            if (shot == null || !shot.IsLaunchedProjectile)
            {
                continue;
            }

            Transform follow;
            Vector3 start;
            if (!shot.TryResolveSpawn(_v2Resolver, _context, out follow, out start))
            {
                start = _context.CasterTransform != null
                    ? _context.CasterTransform.position
                    : transform.position;
            }

            Vector3 aim = VectorWorldUtils.GetCollision(start, _context.TargetTransform);
            float flight = ProjectileFlightTimeCalculator.CalculateL2AccelFlightTimeSeconds(
                Vector3.Distance(start, aim));
            float impactAt =
                _castData.AnimationShotAtSeconds +
                flight +
                ProjectileImpactLifetimeSafetySeconds;
            float remainingFromNow =
                (_castData.StartTime + impactAt) - Time.time;
            requiredLifetime = Mathf.Max(requiredLifetime, remainingFromNow);
        }

        lifeTimeSettings.defaultLifeTime = requiredLifetime;
        lifeTimeSettings.hideTime = Mathf.Min(
            lifeTimeSettings.hideTime,
            lifeTimeSettings.defaultLifeTime);
    }
}
