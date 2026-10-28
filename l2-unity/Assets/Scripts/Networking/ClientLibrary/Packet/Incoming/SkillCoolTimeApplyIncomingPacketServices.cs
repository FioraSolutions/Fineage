using System;

/// <summary>
/// Skill learn wire packet handler entry (#142 batch slice 139).
/// Splits apply out of <see cref="SkillLearnIncomingPacketServices"/> (sub-slice 69).
/// Apply facade (#142 batch slice 154). Re-homes from <see cref="SkillCoolTimeIncomingPacketServices"/>.
/// </summary>
public static class SkillCoolTimeApplyIncomingPacketServices
{
    public static void ApplySkillCoolTime(SkillCoolTimeDto packet)
    {
    }
}
