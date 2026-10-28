using System;

/// <summary>
/// Skill learn wire packet handler entry (#142 batch slice 139).
/// Splits apply out of <see cref="SkillLearnIncomingPacketServices"/> (sub-slice 69).
/// Apply facade (#142 batch slice 154; locator queue slice 258). Re-homes from <see cref="AcquireSkillInfoIncomingPacketServices"/>.
/// </summary>
public static class AcquireSkillInfoApplyIncomingPacketServices
{
    public static void Queue(System.Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyAcquireSkillInfo(AcquireSkillInfoDto packet) =>
        Queue(() =>
        {
            LocatorApplyIncomingPacketServices.SkillLearn.HideWindow();
            LocatorApplyIncomingPacketServices.SkillDesc.AddData(packet);
            LocatorApplyIncomingPacketServices.SkillDesc.ShowWindow();
        });
}
