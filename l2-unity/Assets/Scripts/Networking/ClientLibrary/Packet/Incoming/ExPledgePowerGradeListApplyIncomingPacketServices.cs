using System;

/// <summary>
/// Ex pledge power-grade-list wire packet handler entry (#142 sub-slice 130).
/// Pledge apply facade (#142 batch slice 149; locator queue slice 256). Re-homes from <see cref="ExPledgePowerGradeListIncomingPacketServices"/>. Splits apply out of <see cref="PledgeIncomingPacketServices"/> (sub-slice 60 encapsulation).
/// </summary>
public static class ExPledgePowerGradeListApplyIncomingPacketServices
{
    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyExPledgePowerGradeList(PledgePowerGradeListDto packet) =>
        Queue(() => LocatorApplyIncomingPacketServices.Clan.ShowGradeInfo(packet));

}
