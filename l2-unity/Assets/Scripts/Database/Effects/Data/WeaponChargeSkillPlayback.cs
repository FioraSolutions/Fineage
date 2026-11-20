using UnityEngine;

/// <summary>
/// EffectManager / InterludeSfx playback for <see cref="WeaponChargeSkill"/> ids.
/// Sound and weapon FX are triggered together in one call (no delayed audio queue).
/// </summary>
public static class WeaponChargeSkillPlayback
{
    public static void Play(EffectManager effects, int skillId, Transform weapon, Transform fallback)
    {
        int effectId = WeaponChargeSkill.EffectId(skillId);
        Transform target = weapon != null ? weapon : fallback;
        if (effects == null || effectId <= 0 || target == null)
            return;

        // Interlude: charge SFX starts with the weapon glow — fire audio before effect spawn work.
        PlayChargeSound(skillId, target.position);
        effects.PlayEffect(effectId, target);
    }

    /// <summary>
    /// skillsoundgrp for soulshot/spiritshot charge: cast + shot at activation (HitTime is not used).
    /// </summary>
    public static void PlayChargeSound(int skillId, Vector3 worldPos)
    {
        bool spirit = WeaponChargeSkill.IsSpirit(skillId);
        string castStem = spirit ? "spirits_shot_cast" : "soul_shot_cast";
        string shotStem = spirit ? "soul_shot_shot" : "soul_shot_shot";

        InterludeSfx.Play("Data/Sounds/SkillSound/" + castStem, 0.85f);
        InterludeSfx.Play("Data/Sounds/SkillSound/" + shotStem, 0.85f);
    }
}