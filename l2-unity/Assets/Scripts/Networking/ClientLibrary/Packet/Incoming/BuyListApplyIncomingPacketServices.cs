using System;

/// <summary>
/// Buy list wire packet handler entry (#142 batch slice 148).
/// Splits dealer buy-list apply out of <see cref="GameApplyIncomingPacketServices"/> (sub-slice 84 encapsulation).
/// </summary>
public static class BuyListApplyIncomingPacketServices
{
    public static void Queue(Action action) => MerchantApplyIncomingPacketServices.Queue(action);

    public static void ApplyBuyList(BuyListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            MerchantApplyIncomingPacketServices.Dealer.SetWindowName("Shop");
            MerchantApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Sell");
            MerchantApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Buy");
            MerchantApplyIncomingPacketServices.Dealer.SetProductType(ProductType.BUY);
            MerchantApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, false, packet.ListID);
            MerchantApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            MerchantApplyIncomingPacketServices.Dealer.ShowWindowToCenter();
        });
}
