using UnityEngine;
using System;

public partial class EntityActionVisual
{
    static void PlayUserPhysicalSkill(UserEntity user, MagicSkillUseDto magic)
    {
        if (user == null || user.Identity == null)
            return;

        string trigger = ResolveUserPhysicalSkillTrigger(magic);
        if (string.IsNullOrEmpty(trigger))
            return;

        bool basicJatk = trigger.StartsWith("jatk", System.StringComparison.OrdinalIgnoreCase);
        NetworkAnimationController nac = user.GetAnimatorController();
        Animator animator = nac != null ? nac.GetAnimator() : null;
        if (basicJatk &&
            (user.IsAttackVisualPlaying() || PlayerBasicAttackAnim.IsSwingPlaying(animator, 0.85f)))
        {
            // EntityActionCombatLog.LogCiPawn(user,
                // "Skill SKIP_JATK nick=" + EntityActionCombatLog.NameOf(user) +
                // " skill=" + (magic != null ? magic.SkillId.ToString() : "-") +
                // " lvl=" + (magic != null ? magic.SkillLvl.ToString() : "-") +
                // " trigger=" + trigger +
                // " visLeft=" + user.AttackVisualLeftSec().ToString("F3") +
                // " " + EntityActionCombatLog.AnimDump(user));
            return;
        }

        int objectId = user.Identity.Id;
        if (ControllerPacketServices.Animations != null && magic != null && magic.HitTime > 0)
            ControllerPacketServices.Animations.SetSpTimeAtk(objectId, magic.HitTime);

        string state = trigger + user.WeaponAnim;

        float cycleMs = magic != null && magic.HitTime > 0
            ? magic.HitTime
            : AttackTimingHelper.ResolveAttackCycleMs(user, state);
        user.BeginAttackVisual(cycleMs / 1000f);

        if (ControllerPacketServices.Animations != null)
        {
            ControllerPacketServices.Animations.PlayAnimationTrigger(objectId, trigger);
            // Debug.Log(user.LogTag + " Visual.PhysicalSkill trigger=" + trigger +
                // " state=" + state + " cycleMs=" + cycleMs);
            return;
        }

        PlayUserMeleeAttack(user);
    }

    static string ResolveUserPhysicalSkillTrigger(MagicSkillUseDto magic)
    {
        if (magic == null)
            return UserMeleeVariants[UnityEngine.Random.Range(0, UserMeleeVariants.Length)];

        if (SkillAnimationResolver.TryGetPrimarySpAtkTrigger(magic.SkillId, magic.SkillLvl, out string trigger))
            return trigger;

        Skillgrp grp = magic.SkillGrp;
        if (grp == null && TablePacketServices.SkillGrps != null)
            grp = TablePacketServices.SkillGrps.GetSkill(magic.SkillId, magic.SkillLvl);
        SkillAnimationResolver.LogMissingMixCombo(
            magic.SkillId,
            magic.SkillLvl,
            grp != null ? grp.Animation : null);
        return null;
    }
}
