using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class CompositePrefabEffect
{
    void TryLaunchPartAsProjectileImmediately(CompositePrefabPart part, BaseEffect spawned)
    {
        if (part == null || spawned == null || !CompositeProjectileLaunchHelper.ShouldLaunchImmediately(part))
        {
            return;
        }

        if (_context?.TargetTransform == null || ProjectilePacketServices.Projectiles == null || _launchedProjectileParts.Contains(part))
        {
            return;
        }

        if (CompositeProjectileLaunchHelper.TryLaunch(part, spawned, _context.TargetTransform, _playStartedAt, DebugPrefix))
        {
            _launchedProjectileParts.Add(part);
        }
    }

    void TryLaunchPartAsHomeProjectileImmediately(CompositePrefabPart part, BaseEffect spawned)
    {
        if (part == null || spawned == null ||
            part.homeProjectile == null || !part.homeProjectile.ShouldLaunchImmediately)
        {
            return;
        }

        IHomeProjectileService homeProjectiles = HomeProjectiles;
        if (homeProjectiles == null || _context?.CasterTransform == null || _launchedHomeProjectileParts.Contains(part))
        {
            return;
        }

        if (homeProjectiles.TryLaunchPart(part, spawned, _context))
        {
            _launchedHomeProjectileParts.Add(part);
        }
    }
}
