using System;

/// <summary>
/// Skill learn wire packet handler entry (#142 batch slice 139).
/// Splits apply out of <see cref="SkillLearnIncomingPacketServices"/> (sub-slice 69).
/// Apply facade (#142 batch slice 154). Re-homes from <see cref="AcquireSkillListIncomingPacketServices"/>.
/// </summary>
public static class AcquireSkillListApplyIncomingPacketServices
{
    public static void Queue(System.Action action) => SkillApplyIncomingPacketServices.Queue(action);

    public static void ApplyAcquireSkillList(AcquireSkillListDto packet) =>
        Queue(() =>
        {
            SkillApplyIncomingPacketServices.SkillLearn.AddData(packet.AcquireList);
            SkillApplyIncomingPacketServices.SkillLearn.ShowWindow();
        });
}
