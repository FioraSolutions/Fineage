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

        // Interlude: charge SFX (cast) starts with the weapon glow — shot plays on AttackShot consume.
        PlayChargeActivationSound(skillId, target.position);
        effects.PlayEffect(effectId, target);
    }

    public static void PlayChargeActivationSound(int skillId, Vector3 worldPos)
    {
        SkillCastSound.PlayWeaponChargeActivation(skillId, 1, worldPos);
    }

    public static void PlayChargeImpactSound(int skillId, Vector3 worldPos)
    {
        SkillCastSound.PlayWeaponChargeImpact(skillId, 1, worldPos);
    }
}