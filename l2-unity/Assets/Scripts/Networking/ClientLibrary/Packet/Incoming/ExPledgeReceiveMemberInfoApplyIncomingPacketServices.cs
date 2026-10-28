using System;

/// <summary>
/// Ex pledge receive-member-info wire packet handler entry (#142 sub-slice 131).
/// Pledge apply facade (#142 batch slice 149; locator queue slice 256). Re-homes from <see cref="ExPledgeReceiveMemberInfoIncomingPacketServices"/>. Splits apply out of <see cref="PledgeIncomingPacketServices"/> (sub-slice 60 encapsulation).
/// </summary>
public static class ExPledgeReceiveMemberInfoApplyIncomingPacketServices
{
    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyExPledgeReceiveMemberInfo(PledgeReceiveMemberInfoDto packet) =>
        Queue(() => LocatorApplyIncomingPacketServices.Clan.UpdateDetailedInfo(packet));

}
