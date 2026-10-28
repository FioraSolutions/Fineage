using System;

/// <summary>
/// Pledge show-member-list-delete wire packet handler entry (#142 sub-slice 127).
/// Pledge apply facade (#142 batch slice 149). Re-homes from <see cref="PledgeShowMemberListDeleteIncomingPacketServices"/>. Splits apply out of <see cref="PledgeIncomingPacketServices"/> (sub-slice 60 encapsulation).
/// </summary>
public static class PledgeShowMemberListDeleteApplyIncomingPacketServices
{
    public static void Queue(Action action) => PledgeApplyIncomingPacketServices.Queue(action);

    public static void ApplyPledgeShowMemberListDelete(PledgeShowMemberListDeleteDto packet) =>
        Queue(() => PledgeApplyIncomingPacketServices.Clan.DeleteMemberData(packet));
}
