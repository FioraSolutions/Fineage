using System;

/// <summary>
/// Sell list wire packet handler entry (#142 batch slice 148).
/// Splits sell-list apply out of <see cref="GameApplyIncomingPacketServices"/> (sub-slice 84 encapsulation).
/// </summary>
public static class SellListApplyIncomingPacketServices
{
    public static void Queue(Action action) => GameApplyIncomingPacketServices.Queue(action);

    public static void ApplySellList(SellListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameApplyIncomingPacketServices.Dealer.SetWindowName("Shop");
            GameApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            GameApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Sell");
            GameApplyIncomingPacketServices.Dealer.SetProductType(ProductType.SELL);
            GameApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, true, packet.ListID);
            GameApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameApplyIncomingPacketServices.Dealer.ShowWindow();
        });
}
