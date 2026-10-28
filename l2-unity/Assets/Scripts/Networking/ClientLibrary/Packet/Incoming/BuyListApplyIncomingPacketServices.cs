using System;

/// <summary>
/// Buy list wire packet handler entry (#142 batch slice 148).
/// Splits dealer buy-list apply out of <see cref="MerchantListApplyIncomingPacketServices"/> (sub-slice 84 encapsulation).
/// </summary>
public static class BuyListApplyIncomingPacketServices
{
    public static void Queue(Action action) => MerchantListApplyIncomingPacketServices.Queue(action);

    public static void ApplyBuyList(BuyListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            MerchantListApplyIncomingPacketServices.Dealer.SetWindowName("Shop");
            MerchantListApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Sell");
            MerchantListApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Buy");
            MerchantListApplyIncomingPacketServices.Dealer.SetProductType(ProductType.BUY);
            MerchantListApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, false, packet.ListID);
            MerchantListApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            MerchantListApplyIncomingPacketServices.Dealer.ShowWindowToCenter();
        });
}
