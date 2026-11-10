using UnityEngine;

/// <summary>
/// EffectManager / InterludeSfx playback for <see cref="WeaponChargeSkill"/> ids.
/// </summary>
public static class WeaponChargeSkillPlayback
{
    public static void Play(EffectManager effects, int skillId, Transform weapon, Transform fallback)
    {
        int effectId = WeaponChargeSkill.EffectId(skillId);
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
        string stem = WeaponChargeSkill.IsSpirit(skillId) ? "spirits_shot_cast" : "soul_shot_cast";
        if (InterludeSfx.Play("Data/Sounds/SkillSound/" + stem, 0.85f))
            return;
        if (!WeaponChargeSkill.IsSpirit(skillId))
            InterludeSfx.Play("Data/Sounds/SkillSound/soul_shot_shot", 0.85f);
    }
}
