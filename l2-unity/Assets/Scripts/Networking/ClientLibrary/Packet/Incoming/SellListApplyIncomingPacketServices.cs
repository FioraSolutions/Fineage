using System;

/// <summary>
/// Sell list wire packet handler entry (#142 batch slice 148).
/// Splits sell-list apply out of merchant list hub (sub-slice 84 encapsulation).
/// </summary>
public static class SellListApplyIncomingPacketServices
{
    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplySellList(SellListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            LocatorApplyIncomingPacketServices.Dealer.SetWindowName("Shop");
            LocatorApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            LocatorApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Sell");
            LocatorApplyIncomingPacketServices.Dealer.SetProductType(ProductType.SELL);
            LocatorApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, true, packet.ListID);
            LocatorApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            LocatorApplyIncomingPacketServices.Dealer.ShowWindow();
        });
}
