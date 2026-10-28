using System;

/// <summary>
/// Buy-list seed (manor estate) wire packet handler entry (#142 sub-slice 115).
/// Splits dealer buy-list seed apply out of <see cref="ShopListIncomingPacketServices"/> (sub-slice 84 encapsulation).
/// </summary>
public static class BuyListSeedIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplyBuyListSeed(BuyListSeedDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameIncomingPacketServices.Dealer.SetWindowName("Estate");
            GameIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Sale");
            GameIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Purchase");
            GameIncomingPacketServices.Dealer.SetProductType(ProductType.BUY_SEED);
            GameIncomingPacketServices.Dealer.UpdateBuyData(packet.List, true, packet.ManorId);
            GameIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameIncomingPacketServices.Dealer.ShowWindow();
        });
}
