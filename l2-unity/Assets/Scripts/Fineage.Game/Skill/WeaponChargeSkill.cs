using UnityEngine;

/// <summary>
/// Soulshot and spiritshot skill ids that play a weapon charge.
/// Grades match Mobius item_skill tables (NG + D…S). Blessed Spiritshot shares the
/// spirit visual row 2047 with normal Spiritshot.
/// Playback stays on WeaponChargeSkillPlayback (EffectManager / InterludeSfx).
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

    public static int SoulshotSkillIdForGrade(ItemGrade grade)
    {
        int index = grade switch
        {
            ItemGrade.none => 0,
            ItemGrade.d => 1,
            ItemGrade.c => 2,
            ItemGrade.b => 3,
            ItemGrade.a => 4,
            ItemGrade.s => 5,
            _ => 0
        };
        return SoulshotSkills[Mathf.Clamp(index, 0, SoulshotSkills.Length - 1)];
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
