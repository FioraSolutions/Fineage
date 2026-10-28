using System;

/// <summary>
/// Pledge status-changed wire packet handler entry (#142 sub-slice 123).
/// Splits pledge status-changed apply out of <see cref="PledgeIncomingPacketServices"/> (sub-slice 60 encapsulation).
/// </summary>
public static class PledgeStatusChangedIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplyPledgeStatusChanged(PledgeStatusChangedDto packet) =>
        Queue(() => GameIncomingPacketServices.Clan.UpdateClanIdInfo(packet));
}
