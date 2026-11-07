using UnityEngine;

/// <summary>
/// Single entry for Interlude <c>MagicSkillUse</c> presentation on the local player path.
/// Packet semantics: Mobius <c>Creature.beginCast</c> — see Docs/agent-tips/magic-skill-use-flow.md.
/// </summary>
public static class MagicSkillPresentation
{
    public enum Kind
    {
        Charge,
        Potion,
        Cast,
    }

    public static Kind Classify(MagicSkillUseDto dto)
    {
        if (dto == null)
            return Kind.Cast;
        if (WeaponChargeSkill.IsCharge(dto.SkillId))
            return Kind.Charge;
        if (SetupDurationHelper.IsUsePotion(dto))
            return Kind.Potion;
        return Kind.Cast;
    }

    /// <summary>
    /// After world-apply cooldown UI. Owns stop-move, SFX schedule, cast bar, intention.
    /// Charge / potion never enter the cast channel.
    /// </summary>
    public static void PresentLocal(Entity entity, MagicSkillUseDto dto)
    {
        if (entity == null || dto == null)
            return;

        switch (Classify(dto))
        {
            case Kind.Charge:
                EntityActionSkill.TryApplyWeaponCharge(entity, dto);
                return;
            case Kind.Potion:
                SetupDurationHelper.PlayPotionEffect(entity, dto);
                return;
        }

        AnimChannel.Begin(AnimChannel.Kind.Cast);

        CastMoveLock.Begin(dto.HitTime);
        LocalCastStop.HaltForCast();

        SkillCastSound.OnMagicSkillUse(entity, dto);

        if (entity == PlayerEntity.Instance)
        {
            try { CastBarOverlay.Begin(dto.HitTime); }
            catch (System.Exception bar) { Debug.LogWarning("[CastBar] " + bar.Message); }
        }

        if (PlayerStateMachine.Instance == null)
            return;

        Skillgrp grp = dto.SkillGrp;
        if (grp == null && SkillgrpTable.Instance != null)
            grp = SkillgrpTable.Instance.GetSkill(dto.SkillId, dto.SkillLvl);

        bool isMagic = grp == null || grp.IsMagic == 1;
        PlayerStateMachine.Instance.ChangeIntention(
            isMagic ? Intention.INTENTION_MAGIC_ATTACK : Intention.INTENTION_PHYSICAL_SKILLS_ATTACK,
            dto);
    }

    /// <summary>Call when cast playable / skill runner finishes or is cancelled.</summary>
    public static void EndCastChannel()
    {
        AnimChannel.End(AnimChannel.Kind.Cast);
        CastMoveLock.Clear();
    }
}
