using System;

/// <summary>
/// Skill learn wire packet handler entry (#142 batch slice 139).
/// Splits apply out of <see cref="SkillLearnIncomingPacketServices"/> (sub-slice 69).
/// </summary>
public static class AcquireSkillListIncomingPacketServices
{
    public static void Queue(System.Action action) => UiIncomingPacketServices.Queue(action);

    public static void ApplyAcquireSkillList(AcquireSkillListDto packet) =>
        Queue(() =>
        {
            UiIncomingPacketServices.SkillLearn.AddData(packet.AcquireList);
            UiIncomingPacketServices.SkillLearn.ShowWindow();
        });
}
