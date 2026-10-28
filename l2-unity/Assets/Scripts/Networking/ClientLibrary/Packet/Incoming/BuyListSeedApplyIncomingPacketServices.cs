using System;

/// <summary>
/// Buy-list seed (manor estate) wire packet handler entry (#142 batch slice 148).
/// Splits dealer buy-list seed apply out of <see cref="MerchantListApplyIncomingPacketServices"/> (sub-slice 84 encapsulation).
/// </summary>
public static class BuyListSeedApplyIncomingPacketServices
{
    public static void Queue(Action action) => GameApplyIncomingPacketServices.Queue(action);

    public static void ApplyBuyListSeed(BuyListSeedDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameApplyIncomingPacketServices.Dealer.SetWindowName("Estate");
            GameApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Sale");
            GameApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Purchase");
            GameApplyIncomingPacketServices.Dealer.SetProductType(ProductType.BUY_SEED);
            GameApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.List, true, packet.ManorId);
            GameApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameApplyIncomingPacketServices.Dealer.ShowWindow();
        });
}
