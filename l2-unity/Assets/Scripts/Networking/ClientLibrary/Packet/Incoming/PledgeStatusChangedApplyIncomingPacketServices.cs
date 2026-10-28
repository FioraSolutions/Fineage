using System;

/// <summary>
/// Pledge status-changed wire packet handler entry (#142 sub-slice 123).
/// Pledge apply facade (#142 batch slice 149). Re-homes from <see cref="PledgeStatusChangedIncomingPacketServices"/>. Splits pledge status-changed apply out of <see cref="PledgeIncomingPacketServices"/> (sub-slice 60 encapsulation).
/// </summary>
public static class PledgeStatusChangedApplyIncomingPacketServices
{
    public static void Queue(Action action) => PledgeApplyIncomingPacketServices.Queue(action);

    public static void ApplyPledgeStatusChanged(PledgeStatusChangedDto packet) =>
        Queue(() => PledgeApplyIncomingPacketServices.Clan.UpdateClanIdInfo(packet));
}
