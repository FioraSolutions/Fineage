using System;

/// <summary>
/// Pledge show-member-list-delete wire packet handler entry (#142 sub-slice 127).
/// Splits apply out of <see cref="PledgeIncomingPacketServices"/> (sub-slice 60 encapsulation).
/// </summary>
public static class PledgeShowMemberListDeleteIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplyPledgeShowMemberListDelete(PledgeShowMemberListDeleteDto packet) =>
        Queue(() => GameIncomingPacketServices.Clan.DeleteMemberData(packet));
