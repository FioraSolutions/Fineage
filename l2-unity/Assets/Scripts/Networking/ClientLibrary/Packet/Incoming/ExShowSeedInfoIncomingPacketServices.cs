using System;

/// <summary>
/// Ex show-seed-info wire packet handler entry (#142 batch slice 138).
/// Splits apply out of <see cref="ManorIncomingPacketServices"/> (sub-slice 64).
/// </summary>
public static class ExShowSeedInfoIncomingPacketServices
{
    public static void Queue(Action action) => WorldIncomingPacketServices.Queue(action);

    public static void ApplyExShowSeedInfo(ExShowSeedInfoDto packet) =>
        Queue(() =>
        {
            WorldIncomingPacketServices.SeedInfo.ShowWindowActiveTabSeed();
            WorldIncomingPacketServices.SeedInfo.SetDataSeedInfo(packet.List);
        });
