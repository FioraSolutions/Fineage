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

    static bool IsWorldNpcOrMonster(Entity entity)
    {
        return entity is MonsterEntity || entity is NpcEntity;
    }

    static bool IsSkillAnimationNone(MagicSkillUseDto magic)
    {
        if (magic == null)
            return true;
        if (magic.SkillGrp == null)
            return true;
        string anim = magic.SkillGrp.Animation;
        return string.IsNullOrWhiteSpace(anim) ||
               string.Equals(anim, "None", StringComparison.OrdinalIgnoreCase);
    }

    static AnimationCombo GetCombo(MagicSkillUseDto magic)
    {
        if (magic == null || TablePacketServices.SkillGrps == null)
            return null;
        return TablePacketServices.SkillGrps.GetAnimComboBySkillId(magic.SkillId, magic.SkillLvl);
    }

    static AnimationEventsBase ResolveEvents(Entity entity)
    {
        if (entity == null || entity.Identity == null || ControllerPacketServices.Animations == null)
            return null;
        return ControllerPacketServices.Animations.GetAnimationEvents(entity.Identity.Id);
    }

    static bool IsMagicSkill(MagicSkillUseDto magic)
    {
        return magic != null && magic.SkillGrp != null && magic.SkillGrp.IsMagic == 1;
    }

}
