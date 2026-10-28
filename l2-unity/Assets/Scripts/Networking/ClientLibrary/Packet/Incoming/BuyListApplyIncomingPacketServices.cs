using System;

/// <summary>
/// Buy list wire packet handler entry (#142 batch slice 148).
/// Splits dealer buy-list apply out of <see cref="MerchantListApplyIncomingPacketServices"/> (sub-slice 84 encapsulation).
/// </summary>
public static class BuyListApplyIncomingPacketServices
{
    public static void Queue(Action action) => GameApplyIncomingPacketServices.Queue(action);

    public static void ApplyBuyList(BuyListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameApplyIncomingPacketServices.Dealer.SetWindowName("Shop");
            GameApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Sell");
            GameApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Buy");
            GameApplyIncomingPacketServices.Dealer.SetProductType(ProductType.BUY);
            GameApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, false, packet.ListID);
            GameApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameApplyIncomingPacketServices.Dealer.ShowWindowToCenter();
        });
}
