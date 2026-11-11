using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public partial class CompositeEffectV2 : CompositePrefabEffect
{
    protected override void OnTimedCompositeDestroy()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        LogV2TaDestroyState("before flush");
#endif
        // Launch flying orbs before DestroyOwnedV2Parts (which would kill them).
        // Do NOT spawn late cast FX (cs / OnAnimationShoot independents) here — that
        // fires the "hit flash" on the caster at HitTime when AnimationShoot was missed.
        RefreshV2Context();
        ProcessV2ShootShotsOnly("destroy-flush");
        FlushPendingDelayedIndependentParts();
        FlushPendingHitAtCastEnd();
        UnsubscribeV2Shoot();
        UnsubscribeV2Hit();
        StopAndClearCoroutine(ref _v2PendingRoutine);
        StopAndClearCoroutine(ref _v2ShootWatchdogRoutine);
        DestroyOwnedV2Parts();
        ClearV2Runtime();
        base.OnTimedCompositeDestroy();
    }

    void DestroyOwnedV2Parts()
    {
        foreach (KeyValuePair<CompositePart, BaseEffect> pair in _spawned)
        {
            if (pair.Value == null ||
                _launchedProjectiles.Contains(pair.Key) ||
                (pair.Key != null && pair.Key.OutlivesComposite))
            {
                continue;
            }

            Destroy(pair.Value.gameObject);
        }
    }
}
