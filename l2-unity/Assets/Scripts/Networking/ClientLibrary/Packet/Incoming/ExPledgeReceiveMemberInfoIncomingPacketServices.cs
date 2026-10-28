using System;

/// <summary>
/// Ex pledge receive-member-info wire packet handler entry (#142 sub-slice 131).
/// Splits apply out of <see cref="PledgeIncomingPacketServices"/> (sub-slice 60 encapsulation).
/// </summary>
public static class ExPledgeReceiveMemberInfoIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplyExPledgeReceiveMemberInfo(PledgeReceiveMemberInfoDto packet) =>
        Queue(() => GameIncomingPacketServices.Clan.UpdateDetailedInfo(packet));
