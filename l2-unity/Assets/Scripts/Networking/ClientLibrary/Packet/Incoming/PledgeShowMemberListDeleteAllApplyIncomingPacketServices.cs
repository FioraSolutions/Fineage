using System;

/// <summary>
/// Pledge show-member-list-delete-all wire packet handler entry (#142 sub-slice 126).
/// Pledge apply facade (#142 batch slice 149). Re-homes from <see cref="PledgeShowMemberListDeleteAllIncomingPacketServices"/>. Splits member-list-delete-all apply out of <see cref="PledgeIncomingPacketServices"/> (sub-slice 60 encapsulation).
/// </summary>
public static class PledgeShowMemberListDeleteAllApplyIncomingPacketServices
{
    public static void Queue(Action action) => PledgeApplyIncomingPacketServices.Queue(action);

    public static void ApplyPledgeShowMemberListDeleteAll() =>
        Queue(() => PledgeApplyIncomingPacketServices.Clan.DeleteMemberData());
}
