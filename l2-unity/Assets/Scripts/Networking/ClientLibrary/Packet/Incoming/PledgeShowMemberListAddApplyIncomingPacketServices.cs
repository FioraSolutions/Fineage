using System;

/// <summary>
/// Pledge show-member-list-add wire packet handler entry (#142 sub-slice 124).
/// Pledge apply facade (#142 batch slice 149). Re-homes from <see cref="PledgeShowMemberListAddIncomingPacketServices"/>. Splits member-list-add apply out of <see cref="PledgeIncomingPacketServices"/> (sub-slice 60 encapsulation).
/// </summary>
public static class PledgeShowMemberListAddApplyIncomingPacketServices
{
    public static void Queue(Action action) => PledgeApplyIncomingPacketServices.Queue(action);

    public static void ApplyPledgeShowMemberListAdd(PledgeShowMemberListAddDto packet) =>
        Queue(() => PledgeApplyIncomingPacketServices.Clan.AddMemberData(packet));
}
