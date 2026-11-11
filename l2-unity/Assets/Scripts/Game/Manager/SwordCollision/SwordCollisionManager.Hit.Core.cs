using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class SwordCollisionManager
{
    private void LateUpdate()
    {
        // --- OLD wall-clock Hit fire. Disabled — melee uses EmitHitFromAttackShot. ---
        // if (_activeTimedHits.Count == 0) return;
        //
        // float now = Time.time;
        // for (int i = _activeTimedHits.Count - 1; i >= 0; i--)
        // {
        //     TimedSwordHit timed = _activeTimedHits[i];
        //     Transform swordBase = timed.Tracked.basePt;
        //     Transform target = timed.Tracked.target;
        //     if (swordBase == null || target == null)
        //     {
        //         UnregisterSword(swordBase);
        //         continue;
        //     }
        //
        //     if (timed.Fired || now < timed.HitAtSec) continue;
        //
        //     EmitTimedHit(timed);
        //     timed.Fired = true;
        //     UnregisterSword(swordBase);
        // }
    }

    private bool RegisterHit(Transform swordBase, int targetId)
    {
        if (!_hitRegistry.ContainsKey(swordBase)) _hitRegistry[swordBase] = new HashSet<int>();
        if (_hitRegistry[swordBase].Contains(targetId)) return false;

        _hitRegistry[swordBase].Add(targetId);
        return true;
    }
}
