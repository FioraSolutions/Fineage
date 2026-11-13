using System;
using UnityEngine;
using System.Collections.Generic;

public sealed partial class WorldPacketApply
{
    public void SocialAction(SocialActionDto dto)
    {
        if (dto == null)
            return;
        Entity entity = GetEntity(dto.ObjectId);
        EntityWorldApply apply;
        if (entity == null || !TryResolve(entity, out apply))
            return;
        // Mobius SocialAction — gesture ids 2–14, 15 = level-up (SocialAction.LEVEL_UP).
        apply.OnSocialAction(entity, dto);
    }

    public void ChangeWaitType(ChangeWaitTypeDto dto)
    {
        if (dto == null)
            return;
        Entity entity = GetEntity(dto.ObjectId);
        EntityWorldApply apply;
        if (entity == null || !TryResolve(entity, out apply))
            return;
        apply.OnChangeWaitType(entity, dto);
    }

    public void MagicSkillCanceled(MagicSkillCanceledDto dto)
    {
        if (dto == null)
            return;
        Entity entity = GetEntity(dto.ObjectId);
        EntityWorldApply apply;
        if (entity == null || !TryResolve(entity, out apply))
            return;
        if (entity == DomainPacketServices.LocalPlayer)
            CastBarOverlay.Cancel();
        apply.OnMagicSkillCanceled(entity, dto);
    }

    public void MagicSkillUse(MagicSkillUseDto dto)
    {
        if (dto == null)
            return;



        Entity entity = GetEntity(dto.AttackerObjId);
        if (entity == null)
            entity = LocalCasterOrNull(dto.AttackerObjId);
        if (entity == null)
            return;

        EntityWorldApply apply;
        if (entity == null || !TryResolve(entity, out apply))
        {

            return;
        }



        MagicSkillUseTrace.Begin(dto.SkillId);
        try
        {
            // Local player: presentation is inside PlayerWorldApply → MagicSkillPresentation.
            // CharInfo / NPC: apply only (EntityActionSkill owns remote anim + SkillCastSound there).
            apply.OnMagicSkillUse(entity, dto);
            if (entity != DomainPacketServices.LocalPlayer)
            {
                try { SkillCastSound.OnMagicSkillUse(entity, dto); }
                catch (Exception sfx) { Debug.LogWarning("[SkillCastSound] " + sfx.Message); }
            }
        }
        finally
        {
            MagicSkillUseTrace.End();
        }
    }

    public void MagicSkillLaunched(MagicSkillLaunchedDto dto)
    {
        if (dto == null)
            return;

        Entity entity = GetEntity(dto.ObjectId);
        if (entity == null)
            entity = LocalCasterOrNull(dto.ObjectId);
        if (entity == null)
            return;

        // skillsoundgrp spelleffect_sound_3 (*_explotion) on impact.
        try { SkillCastSound.OnMagicSkillLaunched(entity, dto); }
        catch (Exception sfx) { Debug.LogWarning("[SkillCastSound] launched " + sfx.Message); }
        // UnityReplaySession.OnMagicLaunched(entity, dto);
    }
}
