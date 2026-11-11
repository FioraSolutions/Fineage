using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public partial class CompositeEffectV2 : CompositePrefabEffect
{
    void FlushPendingHitAtCastEnd()
    {
        if (_pendingHitCollider.Count == 0)
        {
            return;
        }

        RefreshV2Context();
        // Impact FX belongs on the target. If we have no target, drop — do not paint the caster.
        if (_context == null ||
            (_context.TargetTransform == null && _context.TargetEntity == null))
        {
            _pendingHitCollider.Clear();
            return;
        }

        EnsureHitPointFromTarget();

        for (int i = 0; i < _pendingHitCollider.Count; i++)
        {
            SpawnV2Part(_pendingHitCollider[i]);
        }

        _pendingHitCollider.Clear();
    }

    void FlushPendingDelayedIndependentParts()
    {
        if (_pendingDelayed.Count == 0)
        {
            return;
        }

        for (int i = _pendingDelayed.Count - 1; i >= 0; i--)
        {
            PendingV2Part pending = _pendingDelayed[i];
            if (pending == null || pending.Part == null)
            {
                _pendingDelayed.RemoveAt(i);
                continue;
            }

            if (!pending.Part.OutlivesComposite)
            {
                continue;
            }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // if (V2TaSpawnLog.Matches(pending.Part))
            {
                // V2TaSpawnLog.Warn(
                    // "FLUSH ON DESTROY part='" + pending.Part.name +
                    // "' was scheduled spawnAt=" + pending.SpawnAtTime.ToString("F3") +
                    // " now=" + Time.time.ToString("F3") +
                    // " remaining=" + (pending.SpawnAtTime - Time.time).ToString("0.###") +
                    // "s — composite died before delayed spawn");
            }
#endif
            SpawnV2Part(pending.Part);
            _pendingDelayed.RemoveAt(i);
        }
    }

    bool IsFromLaunchedV2Projectile(Transform attacker)
    {
        foreach (KeyValuePair<CompositePart, BaseEffect> pair in _spawned)
        {
            if (pair.Key == null || !pair.Key.IsLaunchedProjectile || pair.Value == null)
            {
                continue;
            }

            Transform spawnedTransform = pair.Value.transform;
            if (attacker == spawnedTransform || attacker.IsChildOf(spawnedTransform))
            {
                return true;
            }
        }

        return false;
    }

    void StartV2PendingRoutineIfNeeded()
    {
        if (_pendingDelayed.Count > 0 && _v2PendingRoutine == null)
        {
            _v2PendingRoutine = StartCoroutine(SpawnPendingV2Routine());
        }
    }
}
