using UnityEngine;

/// <summary>
/// Soulshot and spiritshot skill ids that play a weapon charge.
/// Grades match Mobius item_skill tables (NG + D…S). Blessed Spiritshot shares the
/// spirit visual row 2047 with normal Spiritshot.
/// </summary>
public static class WeaponChargeSkill
{
    // Mobius ZoneWorldShots: NG, D, C, B, A, S
    static readonly int[] SoulshotSkills = { 2039, 2150, 2151, 2152, 2153, 2154 };
    static readonly int[] SpiritshotSkills = { 2047, 2155, 2156, 2157, 2158, 2159 };
    static readonly int[] BlessedSpiritshotSkills = { 2061, 2160, 2161, 2162, 2163, 2164 };

    public static bool IsCharge(int skillId)
    {
        return EffectId(skillId) > 0;
    }

    public static bool IsSpirit(int skillId)
    {
        return Contains(SpiritshotSkills, skillId) || Contains(BlessedSpiritshotSkills, skillId);
    }

    public static bool IsSoul(int skillId)
    {
        return Contains(SoulshotSkills, skillId);
    }

    public static int EffectId(int skillId)
    {
        if (IsSoul(skillId))
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

    static bool Contains(int[] ids, int skillId)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            if (ids[i] == skillId)
                return true;
        }

        return false;
    }
}
