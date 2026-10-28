using System;

/// <summary>
/// Pledge show-member-list-all wire packet handler entry (#142 sub-slice 125).
/// Splits member-list-all apply out of <see cref="PledgeIncomingPacketServices"/> (sub-slice 60 encapsulation).
/// </summary>
public static class PledgeShowMemberListAllIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplyPledgeShowMemberListAll(PledgeShowMemberListAllDto packet) =>
        Queue(() => GameIncomingPacketServices.Clan.AddClanData(packet));
}
