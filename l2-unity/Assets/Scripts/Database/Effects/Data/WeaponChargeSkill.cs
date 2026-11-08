using UnityEngine;

/// <summary>
/// Soulshot and spiritshot skill ids that play a weapon charge.
/// Spiritshot 2047 and Blessed Spiritshot 2061 share the spirit visual row 2047.
/// </summary>
public static class WeaponChargeSkill
{
    public static bool IsCharge(int skillId)
    {
        return EffectId(skillId) > 0;
    }

    public static bool IsSpirit(int skillId)
    {
        return skillId == (int)SpecialSkillType.SpiritshotNg ||
               skillId == (int)SpecialSkillType.BlessedSpiritshotNg;
    }

    public static int EffectId(int skillId)
    {
        if (skillId == (int)SpecialSkillType.SoulshotNg)
            return (int)SpecialSkillType.SoulshotNg;
        if (IsSpirit(skillId))
            return (int)SpecialSkillType.SpiritshotNg;
        return 0;
    }

    public static void Play(EffectManager effects, int skillId, Transform weapon, Transform fallback)
    {
        int effectId = EffectId(skillId);
        Transform target = weapon != null ? weapon : fallback;
        if (effects == null || effectId <= 0 || target == null)
            return;

        effects.PlayEffect(effectId, target);
        PlayChargeSound(skillId, target.position);
    }

    /// <summary>
    /// Interlude skillsound charge SFX (soul_shot_* / spirits_shot_cast).
    /// Not hero Shot voice — that stays on real MagicSkillUse casts.
    /// </summary>
    public static void PlayChargeSound(int skillId, Vector3 worldPos)
    {
        string stem = IsSpirit(skillId) ? "spirits_shot_cast" : "soul_shot_cast";
        if (InterludeSfx.Play("Data/Sounds/SkillSound/" + stem, 0.85f))
            return;
        if (!IsSpirit(skillId))
            InterludeSfx.Play("Data/Sounds/SkillSound/soul_shot_shot", 0.85f);
    }
}