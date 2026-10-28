using System;

/// <summary>
/// Personal warehouse withdraw-list wire packet handler entry (#142 batch slice 148).
/// Splits WH withdraw-list apply out of merchant list hub (sub-slice 83 encapsulation).
/// </summary>
public static class WhWithdrawListApplyIncomingPacketServices
{
    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyWhWithdrawList(WarehouseWithdrawListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            LocatorApplyIncomingPacketServices.Dealer.SetWindowName("Personal storage");
            LocatorApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Items in Warehouse");
            LocatorApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Items taken away");
            LocatorApplyIncomingPacketServices.Dealer.SetProductType(ProductType.WHWithdrawList);
            LocatorApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, true, -1);
            LocatorApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            LocatorApplyIncomingPacketServices.Dealer.ShowWindow();
        });
}
