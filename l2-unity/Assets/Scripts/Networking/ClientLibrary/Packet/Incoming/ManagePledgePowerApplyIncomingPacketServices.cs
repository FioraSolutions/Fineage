using System;

/// <summary>
/// Manage pledge power wire packet handler entry (#142 sub-slice 129).
/// Pledge apply facade (#142 batch slice 149). Re-homes from <see cref="ManagePledgePowerIncomingPacketServices"/>. Splits apply out of <see cref="PledgeIncomingPacketServices"/> (sub-slice 60 encapsulation).
/// </summary>
public static class ManagePledgePowerApplyIncomingPacketServices
{
    public static void Queue(Action action) => PledgeApplyIncomingPacketServices.Queue(action);

    public static void ApplyManagePledgePower(ManagePledgePowerDto packet) =>
        Queue(() => PledgeApplyIncomingPacketServices.Clan.UpdateDetailedInfo(packet));
