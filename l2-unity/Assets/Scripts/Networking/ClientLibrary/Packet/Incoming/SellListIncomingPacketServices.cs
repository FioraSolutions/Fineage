using System;

/// <summary>
/// Sell list wire packet handler entry (#142 sub-slice 113).
/// Splits sell-list apply out of <see cref="ShopListIncomingPacketServices"/> (sub-slice 84 encapsulation).
/// </summary>
public static class SellListIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplySellList(SellListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameIncomingPacketServices.Dealer.SetWindowName("Shop");
            GameIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            GameIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Sell");
            GameIncomingPacketServices.Dealer.SetProductType(ProductType.SELL);
            GameIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, true, packet.ListID);
            GameIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameIncomingPacketServices.Dealer.ShowWindow();
        });
}
