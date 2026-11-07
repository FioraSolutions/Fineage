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
            try { CastBarOverlay.Begin(dto.HitTime, dto.SkillId); }
            catch (System.Exception bar) { Debug.LogWarning("[CastBar] " + bar.Message); }
        }

        if (PlayerStateMachine.Instance == null)
        {
            Debug.LogWarning(
                "[MagicSkills] PresentLocal no PlayerStateMachine skill=" + dto.SkillId +
                " lv=" + dto.SkillLvl + " hit=" + dto.HitTime);
            return;
        }

        Skillgrp grp = dto.SkillGrp;
        if (grp == null && SkillgrpTable.Instance != null)
            grp = SkillgrpTable.Instance.GetSkill(dto.SkillId, dto.SkillLvl);

        bool isMagic = grp == null || grp.IsMagic == 1;
        Debug.Log(
            "[CastSync] CAST.begin skill=" + dto.SkillId +
            " lv=" + dto.SkillLvl +
            " hitMs=" + dto.HitTime +
            " target=" + dto.TargetId +
            " grp=" + (grp != null) +
            " anim=[" + (grp != null ? grp.Animation : "-") + "]" +
            " intention=" + (isMagic ? "MAGIC" : "PHYS") +
            " t0=" + Time.realtimeSinceStartup.ToString("F3"));
        PlayerStateMachine.Instance.ChangeIntention(
            isMagic ? Intention.INTENTION_MAGIC_ATTACK : Intention.INTENTION_PHYSICAL_SKILLS_ATTACK,
            dto);
    }

    /// <summary>Call when cast playable / skill runner finishes or is cancelled.</summary>
    public static void EndCastChannel()
    {
        float barRem = CastBarOverlay.Remaining01;
        Debug.Log(
            "[CastSync] CAST.end channel t=" + Time.realtimeSinceStartup.ToString("F3") +
            " barRemainPct=" + (barRem < 0f ? -1f : barRem * 100f).ToString("F0"));
        AnimChannel.End(AnimChannel.Kind.Cast);
        CastMoveLock.Clear();
    }
}
