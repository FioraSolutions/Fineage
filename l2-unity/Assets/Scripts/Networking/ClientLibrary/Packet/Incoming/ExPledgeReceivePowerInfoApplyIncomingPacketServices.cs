using System;

/// <summary>
/// Apply facade (#142 batch slice 151; locator queue slice 256). Re-homes from <see cref="ExPledgeReceivePowerInfoIncomingPacketServices"/>.
/// 
/// Ex pledge receive-power-info wire packet handler entry (#142 sub-slice 132).
/// Splits apply out of <see cref="PledgeIncomingPacketServices"/> (sub-slice 60 encapsulation).
/// </summary>
public static class ExPledgeReceivePowerInfoApplyIncomingPacketServices
{
    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyExPledgeReceivePowerInfo(PledgeReceivePowerInfoDto packet) =>
        Queue(() => LocatorApplyIncomingPacketServices.Clan.UpdateDetailedInfo(packet));

}
