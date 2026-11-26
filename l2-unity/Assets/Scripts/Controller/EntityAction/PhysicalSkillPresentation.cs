using UnityEngine;

/// <summary>
/// One physical-skill body animation for every cast.
/// Phases come from skillgrp + AnimationCombo through <see cref="SkillAnimationResolver"/>.
/// A missing combo is an error. The basic-attack swing is not a substitute.
/// </summary>
public static class PhysicalSkillPresentation
{
    public static bool TryPlay(Entity entity, MagicSkillUseDto magic, AnimationEventsBase events)
    {
        if (entity == null || magic == null)
            return false;

        string[] stems = SkillAnimationResolver.GetSpAtkPhaseStems(magic.SkillId, magic.SkillLvl);
        string animTag = magic.SkillGrp != null ? magic.SkillGrp.Animation : null;
        if (stems == null || stems.Length == 0)
        {
            SkillAnimationResolver.LogMissingMixCombo(magic.SkillId, magic.SkillLvl, animTag);
            return false;
        }

        SkillExecutor executor = SkillExecutorPacketServices.Executor;
        if (executor == null)
        {
            Debug.LogError(
                "[SkillAnimation] SkillExecutor missing skillId=" + magic.SkillId +
                " level=" + magic.SkillLvl +
                " animTag=" + (animTag ?? "<null>"));
            return false;
        }

        var combo = new AnimationCombo(animTag ?? string.Empty, stems, string.Empty);
        _ = executor.ExecuteSkill(entity, combo, events);
        return true;
    }
}
