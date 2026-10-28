using System;

/// <summary>
/// Ex show-seed-info wire packet handler entry (#142 batch slice 138).
/// Splits apply out of <see cref="ManorIncomingPacketServices"/> (sub-slice 64).
/// Apply facade (#142 batch slice 156). Re-homes from <see cref="ExShowSeedInfoIncomingPacketServices"/>.
/// </summary>
public static class ExShowSeedInfoApplyIncomingPacketServices
{
    public static void Queue(Action action) => WorldIncomingPacketServices.Queue(action);

    public static void ApplyExShowSeedInfo(ExShowSeedInfoDto packet) =>
        Queue(() =>
        {
            WorldIncomingPacketServices.SeedInfo.ShowWindowActiveTabSeed();
            WorldIncomingPacketServices.SeedInfo.SetDataSeedInfo(packet.List);
        });
