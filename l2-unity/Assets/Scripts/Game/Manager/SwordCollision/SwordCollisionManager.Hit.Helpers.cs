using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class SwordCollisionService
{
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
