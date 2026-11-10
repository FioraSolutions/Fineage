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

    public void UpdateAttackProgress(int entityId, float elapsedMs)
    {
        if (entityId <= 0) return;
        if (!_attackContextsByEntityId.TryGetValue(entityId, out AttackTimingContext context)) return;
        if (elapsedMs < 0f) return;
        context.LastElapsedMs = elapsedMs;
    }

    public void EndAttack(int entityId, int epoch = 0)
    {
        if (entityId <= 0) return;
        if (!_attackContextsByEntityId.TryGetValue(entityId, out AttackTimingContext context)) return;
        if (epoch > 0 && context.Epoch != epoch)
        {
            // Debug.Log(
                // $"{CHAIN_LOG} EndAttack SKIP entityId={entityId} exitEpoch={epoch} liveEpoch={context.Epoch} " +
                // $"(older swing must not clear newer BeginAttack)");
            return;
        }

        _attackContextsByEntityId.Remove(entityId);
    }

    public void EmitHitFromAttackShot(
        int attackerEntityId,
        int targetEntityId,
        Transform swordBase,
        Transform swordTip,
        Transform target)
    {
        if (attackerEntityId <= 0 || swordBase == null || swordTip == null || target == null)
        {
            // Debug.Log(
                // $"[HIT_FX] 3.EmitHitFromAttackShot SKIP bad args " +
                // $"attackerId={attackerEntityId} baseNull={swordBase == null} " +
                // $"tipNull={swordTip == null} targetNull={target == null}");
            return;
        }

        int resolvedTargetId = targetEntityId;
        int epoch = 0;

        if (_attackContextsByEntityId.TryGetValue(attackerEntityId, out AttackTimingContext ctx))
        {
            if (ctx.AttackShotFired)
            {
                // Debug.Log(
                    // $"[HIT_FX] 3.EmitHitFromAttackShot SKIP already fired " +
                    // $"epoch={ctx.Epoch} attackerId={attackerEntityId}");
                return;
            }

            ctx.AttackShotFired = true;
            if (targetEntityId > 0)
            {
                ctx.TargetEntityId = targetEntityId;
            }

            resolvedTargetId = ctx.TargetEntityId > 0 ? ctx.TargetEntityId : targetEntityId;
            epoch = ctx.Epoch;
        }
        else
        {
            // BeginAttack missing/cleared (EndAttack race) — still play Hit FX from anim event.
            // Debug.Log(
                // $"[HIT_FX] 3.EmitHitFromAttackShot NO_CONTEXT still emit " +
                // $"attackerId={attackerEntityId} targetId={targetEntityId} " +
                // $"(BeginAttack absent — AttackShot drives FX)");
        }

        ResetHitRegistry(swordBase);
        var tracked = new TrackedSword(swordBase, swordTip, target, 0f);
        var timed = new TimedSwordHit(tracked, attackerEntityId, resolvedTargetId, Time.time, Time.time);
        EmitTimedHit(timed);

        // Debug.Log(
            // $"[HIT_FX] 3.EmitHitFromAttackShot OK frame={Time.frameCount} t={Time.time:F3} " +
            // $"attackerId={attackerEntityId} epoch={epoch} sword={swordBase.name} target={target.name}");
    }

}
