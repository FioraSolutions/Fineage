using System;
using System.Collections.Generic;
using UnityEngine;

public partial class EntityActionSkill
{
    static string ResolvePlayableNpcSkillSequence(Entity entity, string sequence)
    {
        if (entity == null ||
            string.IsNullOrEmpty(sequence) ||
            !sequence.StartsWith("spatk", StringComparison.OrdinalIgnoreCase))
        {
            return sequence;
        }

        Animator animator = entity.Animator != null
            ? entity.Animator
            : entity.GetComponentInChildren<Animator>(true);
        if (animator == null)
            return sequence;

        string stateName = sequence;
        if (MonsterAnim.TryResolve(sequence, out string resolved, out _))
            stateName = resolved;
        if (animator.HasState(0, Animator.StringToHash(stateName)))
            return sequence;

        const string fallback = MonsterAnim.Atk01;
        if (!animator.HasState(0, Animator.StringToHash(fallback)))
            return sequence;

        // Debug.LogWarning(
            // "[MOBSKILL] Missing animation state nick=" + EntityActionCombatLog.NameOf(entity) +
            // " npcId=" + (entity.Identity != null ? entity.Identity.NpcId : 0) +
            // " requested=" + sequence +
            // " fallback=" + fallback);
        return fallback;
    }

    public static string AnimatorStateName(Animator animator)
    {
        if (animator == null)
            return "none";

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        if (animator.IsInTransition(0))
        {
            AnimatorStateInfo next = animator.GetNextAnimatorStateInfo(0);
            return StateName(state) + "->" + StateName(next);
        }

        return StateName(state);
    }

    static string StateName(AnimatorStateInfo state)
    {
        if (state.IsName("atk01")) return "atk01";
        if (state.IsName("atkwait")) return "atkwait";
        if (state.IsName("wait")) return "wait";
        if (state.IsName("walk")) return "walk";
        if (state.IsName("run")) return "run";
        if (state.IsName("spatk01")) return "spatk01";
        if (state.IsName("death")) return "death";
        return "other";
    }

    static bool IsWorldNpcOrMonster(Entity entity) =>
        EntityActionVisual.UsesCreatureAnim(entity);
}
