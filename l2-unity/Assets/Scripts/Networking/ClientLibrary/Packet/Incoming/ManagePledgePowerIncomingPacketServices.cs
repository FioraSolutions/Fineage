using System;

/// <summary>
/// Manage pledge power wire packet handler entry (#142 sub-slice 129).
/// Splits apply out of <see cref="PledgeIncomingPacketServices"/> (sub-slice 60 encapsulation).
/// </summary>
public static class ManagePledgePowerIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplyManagePledgePower(ManagePledgePowerDto packet) =>
        Queue(() => GameIncomingPacketServices.Clan.UpdateDetailedInfo(packet));
