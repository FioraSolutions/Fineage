using System.Globalization;
using UnityEngine;

/// <summary>
/// Single place that decides which effect art a skill cast plays.
/// </summary>
/// <remarks>
/// The client used to play art only for Power Strike (3) and otherwise stayed silent, so drops like
/// Deadly Blow (263) and Lethal Blow (344) animated the swing with nothing on screen.
/// The skill id stays the primary key because that is what the effect database is built on; when an
/// id has no row, the skillgrp <c>skill_visual_effect</c> column is tried next (it can point elsewhere,
/// e.g. 104 → 78). Ids nobody knows are skipped silently instead of logging an abort warning on
/// every cast.
/// </remarks>
public static class SkillVisualEffectDispatch
{
    /// <summary>
    /// Effect id to play: the skill id when registered, else the skillgrp visual-effect id, else 0.
    /// </summary>
    public static int ResolveEffectId(MagicSkillUseDto magic, EffectManager effects)
    {
        if (magic == null || effects == null)
            return 0;

        if (effects.HasEffect(magic.SkillId))
            return magic.SkillId;

        Skillgrp grp = magic.SkillGrp;
        if (grp == null && TablePacketServices.SkillGrps != null)
            grp = TablePacketServices.SkillGrps.GetSkill(magic.SkillId, magic.SkillLvl);

        int fromTable = ParseNumericEffectId(grp != null ? grp.SkillVisualEffect : null);
        return fromTable > 0 && effects.HasEffect(fromTable) ? fromTable : 0;
    }

    /// <summary>
    /// Plays the skill art when something can be resolved for it. With an animation combo the effect
    /// rides the weapon and waits for the hit frame; without one it still plays, because a missing
    /// animation row must not also remove the art.
    /// </summary>
    public static void PlayIfKnown(Entity entity, MagicSkillUseDto magic, AnimationCombo combo = null)
    {
        EffectManager effects = ControllerPacketServices.Effects;
        if (effects == null || entity == null || magic == null)
            return;

        int effectId = ResolveEffectId(magic, effects);
        if (effectId <= 0)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("[SkillVFX] skip skill=" + magic.SkillId + " lv=" + magic.SkillLvl +
                      " (no effects row)");
#endif
            return;
        }

        if (combo != null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("[SkillVFX] play skill=" + magic.SkillId + " effect=" + effectId +
                      " hitMs=" + magic.HitTime + " synced=true");
#endif
            effects.PlayEffectSyncedToSkillAnimation(effectId, entity, magic.HitTime, combo);
            return;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log("[SkillVFX] play skill=" + magic.SkillId + " effect=" + effectId +
                  " hitMs=" + magic.HitTime + " synced=false");
#endif
        effects.PlayEffect(effectId, entity.transform, entity.GetMagicCastData());
    }

    /// <summary>
    /// Parses <c>skill_visual_effect</c> rows such as "[263]" or "[11514_a]". Named rows (and the
    /// "[None]" placeholder) return 0 so the caller falls back to the skill id.
    /// </summary>
    static int ParseNumericEffectId(string raw)
    {
        if (string.IsNullOrEmpty(raw))
            return 0;

        string trimmed = raw.Trim().Trim('[', ']');
        if (trimmed.Length == 0)
            return 0;
        if (string.Equals(trimmed, "None", System.StringComparison.OrdinalIgnoreCase))
            return 0;

        foreach (char c in trimmed)
        {
            if (!char.IsDigit(c))
                return 0;
        }

        return int.TryParse(trimmed, NumberStyles.Integer, CultureInfo.InvariantCulture, out int id)
            ? id
            : 0;
    }
}
