using System;

/// <summary>
/// Pledge show-member-list-all wire packet handler entry (#142 sub-slice 125).
/// Pledge apply facade (#142 batch slice 149; locator queue slice 256). Re-homes from <see cref="PledgeShowMemberListAllIncomingPacketServices"/>. Splits member-list-all apply out of <see cref="PledgeIncomingPacketServices"/> (sub-slice 60 encapsulation).
/// </summary>
public static class PledgeShowMemberListAllApplyIncomingPacketServices
{
    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyPledgeShowMemberListAll(PledgeShowMemberListAllDto packet) =>
        Queue(() => LocatorApplyIncomingPacketServices.Clan.AddClanData(packet));
}
