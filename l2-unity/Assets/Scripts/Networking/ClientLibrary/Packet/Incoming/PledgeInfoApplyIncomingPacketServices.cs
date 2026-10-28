using System;

/// <summary>
/// Pledge info wire packet handler entry (#142 sub-slice 122).
/// Pledge apply facade (#142 batch slice 149). Re-homes from <see cref="PledgeInfoIncomingPacketServices"/>. Splits pledge-info apply out of <see cref="PledgeIncomingPacketServices"/> (sub-slice 60 encapsulation).
/// </summary>
public static class PledgeInfoApplyIncomingPacketServices
{
    public static void Queue(Action action) => GameApplyIncomingPacketServices.Queue(action);

    public static void ApplyPledgeInfo(PledgeInfoDto packet) =>
        Queue(() => GameApplyIncomingPacketServices.Clan.UpdatePledge(packet));
}
