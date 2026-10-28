using System;

/// <summary>
/// Ex show-sell-crop-list wire packet handler entry (#142 batch slice 138).
/// Splits apply out of <see cref="ManorIncomingPacketServices"/> (sub-slice 64).
/// Apply facade (#142 batch slice 156; locator queue slice 260). Re-homes from <see cref="ExShowSellCropListIncomingPacketServices"/>.
/// </summary>
public static class ExShowSellCropListApplyIncomingPacketServices
{
    public static SellCropListWindow SellCrop => LocatorApplyIncomingPacketServices.SellCrop;

    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyExShowSellCropList(ExShowSellCropListDto packet) =>
        Queue(() =>
        {
            SellCrop.ShowWindow();
            SellCrop.SetDataTable(packet);
        });
}
