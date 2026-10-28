using System;

/// <summary>
/// Pledge show-member-list-update wire packet handler entry (#142 sub-slice 128).
/// Pledge apply facade (#142 batch slice 149). Re-homes from <see cref="PledgeShowMemberListUpdateIncomingPacketServices"/>. Splits apply out of <see cref="PledgeIncomingPacketServices"/> (sub-slice 60 encapsulation).
/// </summary>
public static class PledgeShowMemberListUpdateApplyIncomingPacketServices
{
    public static void Queue(Action action) => PledgeApplyIncomingPacketServices.Queue(action);

    public static void ApplyPledgeShowMemberListUpdate(PledgeShowMemberListUpdateDto packet) =>
        Queue(() => PledgeApplyIncomingPacketServices.Clan.UpdateMemberData(packet));
}
