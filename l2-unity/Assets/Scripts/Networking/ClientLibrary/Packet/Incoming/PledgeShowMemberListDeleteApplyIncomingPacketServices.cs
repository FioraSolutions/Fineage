using System;

/// <summary>
/// Pledge show-member-list-delete wire packet handler entry (#142 sub-slice 127).
/// Pledge apply facade (#142 batch slice 149; locator queue slice 256). Re-homes from <see cref="PledgeShowMemberListDeleteIncomingPacketServices"/>. Splits apply out of <see cref="PledgeIncomingPacketServices"/> (sub-slice 60 encapsulation).
/// </summary>
public static class PledgeShowMemberListDeleteApplyIncomingPacketServices
{
    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyPledgeShowMemberListDelete(PledgeShowMemberListDeleteDto packet) =>
        Queue(() => LocatorApplyIncomingPacketServices.Clan.DeleteMemberData(packet));

}
