using System;

/// <summary>
/// Skill learn wire packet handler entry (#142 batch slice 139).
/// Splits apply out of <see cref="SkillLearnIncomingPacketServices"/> (sub-slice 69).
/// Apply facade (#142 batch slice 154). Re-homes from <see cref="SkillListIncomingPacketServices"/>.
/// </summary>
public static class SkillListApplyIncomingPacketServices
{
    public static void Queue(System.Action action) => UiApplyIncomingPacketServices.Queue(action);

    public static void ApplySkillList(SkillListDto packet) =>
        Queue(() => SkillApplyIncomingPacketServices.SkillList.UpdateSkillList(packet.Skills));
}
