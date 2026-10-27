using System;

/// <summary>
/// Manor / seed Ex wire packet handler access via WorldIncomingPacketServices (#142 sub-slice 64).
/// </summary>
public static class ManorIncomingPacketServices
{
    public static void Queue(Action action) => WorldIncomingPacketServices.Queue(action);

    public static void ApplyExShowCropInfo(ExShowCropInfoDto packet) =>
        Queue(() =>
        {
            WorldIncomingPacketServices.SeedInfo.SetDataCropInfo(packet.List);
            WorldIncomingPacketServices.SeedInfo.ShowWindow();
        });

    public static void ApplyExShowManorDefaultInfo(ExShowManorDefaultInfoDto packet) =>
        Queue(() =>
        {
            WorldIncomingPacketServices.SeedInfo.SetDataDefaultManorInfo(packet.List);
            WorldIncomingPacketServices.SeedInfo.ShowWindowActiveTabAllDefault();
        });

    public static void ApplyExShowSeedInfo(ExShowSeedInfoDto packet) =>
        Queue(() =>
        {
            WorldIncomingPacketServices.SeedInfo.ShowWindowActiveTabSeed();
            WorldIncomingPacketServices.SeedInfo.SetDataSeedInfo(packet.List);
        });

    public static void ApplyExShowSellCropList(ExShowSellCropListDto packet) =>
        Queue(() =>
        {
            WorldIncomingPacketServices.SellCrop.ShowWindow();
            WorldIncomingPacketServices.SellCrop.SetDataTable(packet);
        });
}
