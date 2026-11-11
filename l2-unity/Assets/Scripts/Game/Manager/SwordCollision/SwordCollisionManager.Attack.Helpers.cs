using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class SwordCollisionManager
{
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
