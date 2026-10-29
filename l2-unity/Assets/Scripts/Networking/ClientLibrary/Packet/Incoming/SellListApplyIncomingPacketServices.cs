using System;

/// <summary>
/// Sell list wire packet handler entry (#142 batch slice 148; locator queue slice 260).
/// Splits sell-list apply out of merchant list hub (sub-slice 84 encapsulation).
/// </summary>
public static class SellListApplyIncomingPacketServices
{
    public static DealerWindow Dealer => LocatorApplyIncomingPacketServices.Dealer;

    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplySellList(SellListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            Dealer.SetWindowName("Shop");
            Dealer.SetHeaderNameSellPanel("Inventory");
            Dealer.SetHeaderNameBuyPanel("Sell");
            Dealer.SetProductType(ProductType.SELL);
            Dealer.UpdateBuyData(packet.Products, true, packet.ListID);
            Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            Dealer.ShowWindow();
        });
}
