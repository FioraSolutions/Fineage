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

    private void EmitTimedHit(TimedSwordHit timed)
    {
        Transform swordBase = timed.Tracked.basePt;
        Transform target = timed.Tracked.target;
        if (swordBase == null || target == null) return;

        Entity attackerEntity = GetEntityById(timed.AttackerEntityId);
        Entity targetEntity = GetEntityById(timed.TargetEntityId);
        Transform attackerAnchor = attackerEntity != null ? attackerEntity.transform : swordBase;
        Transform targetAnchor = targetEntity != null ? targetEntity.transform : target;
        Transform hitAnchor = HitAnchorResolver.ResolveHitAnchor(targetEntity, targetAnchor);
        int targetId = targetAnchor.GetInstanceID();
        if (!RegisterHit(swordBase, targetId))
        {
            // Debug.Log(
                // $"[HIT_FX] 4.EmitTimedHit SKIP RegisterHit duplicate " +
                // $"sword={swordBase.name} target={targetAnchor.name}");
            return;
        }

        Vector3 hitPoint = hitAnchor != null ? hitAnchor.position : targetAnchor.position;
        Vector3 hitDirection = VectorUtils.CalcHitDirection(hitPoint, swordBase.position);

        int listeners = OnHitCollider != null ? OnHitCollider.GetInvocationList().Length : 0;
        // Debug.Log(
            // $"[HIT_FX] 4.EmitTimedHit → OnHitCollider listeners={listeners} " +
            // $"attacker={attackerAnchor.name} target={targetAnchor.name} hitPoint={hitPoint}");

        OnHitCollider?.Invoke(attackerAnchor, targetAnchor, hitPoint, hitDirection);

        float now = Time.time;
        float sinceRegisterMs = (now - timed.StartTimeSec) * 1000f;
        float sinceBeginMs = -1f;
        if (timed.AttackerEntityId > 0 &&
            _attackContextsByEntityId.TryGetValue(timed.AttackerEntityId, out AttackTimingContext ctx))
        {
            sinceBeginMs = (now - ctx.StartTimeSec) * 1000f;
        }

        // Debug.Log(
            // $"[HIT_FX] 4.EmitTimedHit DONE frame={Time.frameCount} t={now:F3} " +
            // $"sinceBeginMs={sinceBeginMs:F1} sinceRegisterMs={sinceRegisterMs:F1}");
        // Debug.Log($"{TIMER_LOG} Hit attacker={attackerAnchor.name} target={targetAnchor.name} elapsedMs={sinceRegisterMs:F1} hitPoint={hitPoint}");
    }

    private Entity GetEntityById(int entityId)
    {
        World world = GameWorld;
        if (entityId <= 0 || world == null)
            return null;
        return world.GetEntityNoLockSync(entityId);
    }

}
