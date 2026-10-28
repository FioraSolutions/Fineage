using System;

/// <summary>
/// Sell list wire packet handler entry (#142 batch slice 148).
/// Splits sell-list apply out of <see cref="GameApplyIncomingPacketServices"/> (sub-slice 84 encapsulation).
/// </summary>
public static class SellListApplyIncomingPacketServices
{
    public static void Queue(Action action) => MerchantApplyIncomingPacketServices.Queue(action);

    public static void ApplySellList(SellListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            MerchantApplyIncomingPacketServices.Dealer.SetWindowName("Shop");
            MerchantApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            MerchantApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Sell");
            MerchantApplyIncomingPacketServices.Dealer.SetProductType(ProductType.SELL);
            MerchantApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, true, packet.ListID);
            MerchantApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            MerchantApplyIncomingPacketServices.Dealer.ShowWindow();
        });
}
