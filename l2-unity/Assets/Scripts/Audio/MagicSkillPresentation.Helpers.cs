using UnityEngine;
using System;

public static partial class MagicSkillPresentation
{
    public static void PresentLocal(Entity entity, MagicSkillUseDto dto)
    {
        if (entity == null || dto == null)
            return;

        switch (Classify(dto))
        {
            case MagicSkillUseKind.Charge:
                EntityActionSkill.TryApplyWeaponCharge(entity, dto);
                return;
            case MagicSkillUseKind.Potion:
                SetupDurationHelper.PlayPotionEffect(entity, dto);
                return;
        }

        AnimChannel.Begin(AnimChannel.Kind.Cast);

        CastMoveLock.Begin(dto.HitTime);
        LocalCastStop.HaltForCast();

        SkillCastSound.OnMagicSkillUse(entity, dto);

        if (entity == DomainPacketServices.LocalPlayer)
        {
            try { CastBarOverlay.Begin(dto.HitTime, dto.SkillId); }
            catch (System.Exception bar) { Debug.LogWarning("[CastBar] " + bar.Message); }
        }

        if (PlayerStatePacketServices.PlayerState == null)
        {
            Debug.LogWarning(
                "[MagicSkills] PresentLocal no PlayerStateMachine skill=" + dto.SkillId +
                " lv=" + dto.SkillLvl + " hit=" + dto.HitTime);
            return;
        }

        Skillgrp grp = dto.SkillGrp;
        if (grp == null && TablePacketServices.SkillGrps != null)
            grp = TablePacketServices.SkillGrps.GetSkill(dto.SkillId, dto.SkillLvl);

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
        PlayerStatePacketServices.PlayerState.ChangeIntention(
            isMagic ? Intention.INTENTION_MAGIC_ATTACK : Intention.INTENTION_PHYSICAL_SKILLS_ATTACK,
            dto);
    }

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
