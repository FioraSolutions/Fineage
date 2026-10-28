using System;

/// <summary>
/// Sell list wire packet handler entry (#142 batch slice 148).
/// Splits sell-list apply out of <see cref="MerchantListApplyIncomingPacketServices"/> (sub-slice 84 encapsulation).
/// </summary>
public static class SellListApplyIncomingPacketServices
{
    public static void Queue(Action action) => MerchantListApplyIncomingPacketServices.Queue(action);

    public static void ApplySellList(SellListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            MerchantListApplyIncomingPacketServices.Dealer.SetWindowName("Shop");
            MerchantListApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            MerchantListApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Sell");
            MerchantListApplyIncomingPacketServices.Dealer.SetProductType(ProductType.SELL);
            MerchantListApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, true, packet.ListID);
            MerchantListApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            MerchantListApplyIncomingPacketServices.Dealer.ShowWindow();
        });
}
