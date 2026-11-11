using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public partial class CompositeEffectV2
{
    void UnsubscribeV2Hit()
    {
        if (ProjectilePacketServices.Projectiles != null && _v2SubscribedHit)
        {
            ProjectilePacketServices.Projectiles.OnHitEffectProjectile -= HandleV2ProjectileHit;
            _v2SubscribedHit = false;
        }
    }

    void HandleV2AnimationShoot(string _)
    {
        _v2DirectShootObserved = true;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        float fromCast = _castData != null ? Time.time - _castData.StartTime : -1f;
        float expected = _castData != null ? _castData.AnimationShotAtSeconds : -1f;
        // Debug.Log(
            // $"{DebugPrefix} [MAGIC_SHOOT_SOURCE] source=AnimationEvent " +
            // $"fromCast={fromCast:F3}s expected={expected:F3}s delta={fromCast - expected:F3}s.");
#endif
        ProcessV2Shoot("direct");
    }
}
