using System;

/// <summary>
/// Buy list wire packet handler entry (#142 batch slice 148; locator queue slice 254).
/// Splits dealer buy-list apply out of merchant list hub (sub-slice 84 encapsulation).
/// </summary>
public static class BuyListApplyIncomingPacketServices
{
    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyBuyList(BuyListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            LocatorApplyIncomingPacketServices.Dealer.SetWindowName("Shop");
            LocatorApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Sell");
            LocatorApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Buy");
            LocatorApplyIncomingPacketServices.Dealer.SetProductType(ProductType.BUY);
            LocatorApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, false, packet.ListID);
            LocatorApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            LocatorApplyIncomingPacketServices.Dealer.ShowWindowToCenter();
        });
}
