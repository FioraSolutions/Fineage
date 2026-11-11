using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public partial class CompositeEffectV2
{
    void SubscribeV2ShootIfNeeded()
    {
        UnsubscribeV2Shoot();
        if (!RequiresV2AnimationShoot())
        {
            return;
        }

        if (_context?.CasterEntity?.Identity == null || Animations == null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // V2TaSpawnLog.Warn(
                // "SHOOT SUBSCRIBE FAIL composite='" + name +
                // "' caster=" + (_context?.CasterEntity != null ? "ok" : "null") +
                // " animations=" + (RenderingPacketServices.Animations != null ? "ok" : "null") +
                // " — waiting for serverTimeToShoot fallback");
#endif
            return;
        }

        int casterId = _context.CasterEntity.Identity.Id;
        _v2AnimationEvents = Animations.GetAnimationEvents(casterId);
        if (_v2AnimationEvents == null)
        {
            // Do not mark subscribed — watchdog / retry can Subscribe again once
            // AnimationManager has registered this caster.
            return;
        }

        if (_v2ShootSources.Contains(_v2AnimationEvents))
        {
            _v2SubscribedShoot = true;
            return;
        }

        _v2AnimationEvents.OnAnimationStartShoot += HandleV2AnimationShoot;
        _v2AnimationEvents.OnAnimationAttackShot += HandleV2AnimationShoot;
        _v2ShootSources.Add(_v2AnimationEvents);
        _v2SubscribedShoot = true;
    }

    void UnsubscribeV2Shoot()
    {
        UnsubscribeShootEventSources(
            _v2ShootSources,
            HandleV2AnimationShoot,
            ref _v2AnimationEvents,
            ref _v2SubscribedShoot);
    }
}
