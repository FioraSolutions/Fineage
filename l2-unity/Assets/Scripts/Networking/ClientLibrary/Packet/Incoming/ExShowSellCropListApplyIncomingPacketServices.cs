using System;

/// <summary>
/// Ex show-sell-crop-list wire packet handler entry (#142 batch slice 138).
/// Splits apply out of <see cref="ManorIncomingPacketServices"/> (sub-slice 64).
/// Apply facade (#142 batch slice 156). Re-homes from <see cref="ExShowSellCropListIncomingPacketServices"/>.
/// </summary>
public static class ExShowSellCropListApplyIncomingPacketServices
{
    public static void Queue(Action action) => ManorApplyIncomingPacketServices.Queue(action);

    public static void ApplyExShowSellCropList(ExShowSellCropListDto packet) =>
        Queue(() =>
        {
            ManorApplyIncomingPacketServices.SellCrop.ShowWindow();
            ManorApplyIncomingPacketServices.SellCrop.SetDataTable(packet);
        });
