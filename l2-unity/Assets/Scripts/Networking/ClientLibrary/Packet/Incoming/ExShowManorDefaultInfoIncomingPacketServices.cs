using System;

/// <summary>
/// Ex show-manor-default-info wire packet handler entry (#142 batch slice 138).
/// Splits apply out of <see cref="ManorIncomingPacketServices"/> (sub-slice 64).
/// </summary>
public static class ExShowManorDefaultInfoIncomingPacketServices
{
    public static void Queue(Action action) => WorldIncomingPacketServices.Queue(action);

    public static void ApplyExShowManorDefaultInfo(ExShowManorDefaultInfoDto packet) =>
        Queue(() =>
        {
            WorldIncomingPacketServices.SeedInfo.SetDataDefaultManorInfo(packet.List);
            WorldIncomingPacketServices.SeedInfo.ShowWindowActiveTabAllDefault();
        });
