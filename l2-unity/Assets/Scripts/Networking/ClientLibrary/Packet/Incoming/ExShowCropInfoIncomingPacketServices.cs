using System;

/// <summary>
/// Ex show-crop-info wire packet handler entry (#142 batch slice 138).
/// Splits apply out of <see cref="ManorIncomingPacketServices"/> (sub-slice 64).
/// </summary>
public static class ExShowCropInfoIncomingPacketServices
{
    public static void Queue(Action action) => WorldIncomingPacketServices.Queue(action);

    public static void ApplyExShowCropInfo(ExShowCropInfoDto packet) =>
        Queue(() =>
        {
            WorldIncomingPacketServices.SeedInfo.SetDataCropInfo(packet.List);
            WorldIncomingPacketServices.SeedInfo.ShowWindow();
        });
