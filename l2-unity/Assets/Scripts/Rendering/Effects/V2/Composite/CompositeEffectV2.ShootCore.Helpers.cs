using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public partial class CompositeEffectV2
{
    bool RequiresV2AnimationShoot()
    {
        if (_v2Parts == null)
        {
            return false;
        }

        for (int i = 0; i < _v2Parts.Length; i++)
        {
            CompositePart part = _v2Parts[i];
            if (part != null && part.WantsAnimationShoot)
            {
                return true;
            }
        }

        return false;
    }

    bool RequiresV2HitCollider()
    {
        if (_v2Parts == null)
        {
            return false;
        }

        for (int i = 0; i < _v2Parts.Length; i++)
        {
            CompositePart part = _v2Parts[i];
            if (part != null && part.spawnTiming == CompositePartSpawnTiming.OnHitCollider)
            {
                return true;
            }
        }

        return false;
    }

    void SubscribeV2HitIfNeeded()
    {
        if (!RequiresV2HitCollider())
        {
            return;
        }

        if (ProjectilePacketServices.Projectiles != null && !_v2SubscribedHit)
        {
            ProjectilePacketServices.Projectiles.OnHitEffectProjectile += HandleV2ProjectileHit;
            _v2SubscribedHit = true;
        }
    }
}
