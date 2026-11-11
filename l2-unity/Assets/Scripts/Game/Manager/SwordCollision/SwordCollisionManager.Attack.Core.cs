using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class SwordCollisionManager
{
    public int BeginAttack(int entityId, int targetEntityId, Transform attacker, Transform target, float attackDurationMs, float hitFraction = DEFAULT_HIT_FRACTION)
    {
        if (entityId <= 0) return 0;

        float normalizedDurationMs = attackDurationMs > 0f ? attackDurationMs : DEFAULT_ATTACK_DURATION_MS;
        float normalizedHitFraction = Mathf.Clamp01(hitFraction);
        int epoch = 1;
        if (_attackContextsByEntityId.TryGetValue(entityId, out AttackTimingContext prev))
        {
            epoch = prev.Epoch + 1;
        }

        _attackContextsByEntityId[entityId] = new AttackTimingContext
        {
            EntityId = entityId,
            TargetEntityId = targetEntityId,
            AttackerTransform = attacker,
            TargetTransform = target,
            StartTimeSec = Time.time,
            DurationMs = normalizedDurationMs,
            HitFraction = normalizedHitFraction,
            LastElapsedMs = 0f,
            Epoch = epoch,
            AttackShotFired = false
        };

        string attackerName = attacker != null ? attacker.name : "null";
        string targetName = target != null ? target.name : "null";
        // Debug.Log(
            // $"{CHAIN_LOG} 1.BeginAttack EVENT_BASED entityId={entityId} epoch={epoch} frame={Time.frameCount} t={Time.time:F3} " +
            // $"attacker={attackerName} target={targetName} durationMs={normalizedDurationMs:F1} " +
            // $"(melee Hit waits for AttackShot anim event; DurationMs kept for jAtk cycle / logs)");
        // Debug.Log($"{TIMER_LOG} BeginAttack entityId={entityId} attacker={attackerName} target={targetName} durationMs={normalizedDurationMs:F1} hitFraction={normalizedHitFraction:F2}");
        return epoch;
    }

    public int BeginAttack(int entityId, Transform attacker, Transform target, float attackDurationMs)
    {
        return BeginAttack(entityId, 0, attacker, target, attackDurationMs);
    }

    public int GetAttackEpoch(int entityId)
    {
        if (entityId <= 0) return 0;
        return _attackContextsByEntityId.TryGetValue(entityId, out AttackTimingContext context) ? context.Epoch : 0;
    }
}
