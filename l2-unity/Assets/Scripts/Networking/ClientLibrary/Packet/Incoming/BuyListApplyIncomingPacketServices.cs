using System;

/// <summary>
/// Buy list wire packet handler entry (#142 batch slice 148).
/// Splits dealer buy-list apply out of <see cref="GameIncomingPacketServices"/> (sub-slice 84 encapsulation).
/// </summary>
public static class BuyListApplyIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplyBuyList(BuyListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameIncomingPacketServices.Dealer.SetWindowName("Shop");
            GameIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Sell");
            GameIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Buy");
            GameIncomingPacketServices.Dealer.SetProductType(ProductType.BUY);
            GameIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, false, packet.ListID);
            GameIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameIncomingPacketServices.Dealer.ShowWindowToCenter();
        });
}
