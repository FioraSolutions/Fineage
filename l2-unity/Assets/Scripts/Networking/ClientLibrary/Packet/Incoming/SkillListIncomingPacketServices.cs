using System;

/// <summary>
/// Skill learn wire packet handler entry (#142 batch slice 139).
/// Splits apply out of <see cref="SkillLearnIncomingPacketServices"/> (sub-slice 69).
/// </summary>
public static class SkillListIncomingPacketServices
{
    public static void Queue(System.Action action) => UiIncomingPacketServices.Queue(action);

    public static void ApplySkillList(SkillListDto packet) =>
        Queue(() => UiIncomingPacketServices.SkillList.UpdateSkillList(packet.Skills));
}
