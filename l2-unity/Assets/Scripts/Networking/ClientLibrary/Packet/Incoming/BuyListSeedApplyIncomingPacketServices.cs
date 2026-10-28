using System;

/// <summary>
/// Buy-list seed (manor estate) wire packet handler entry (#142 batch slice 148).
/// Splits dealer buy-list seed apply out of <see cref="GameApplyIncomingPacketServices"/> (sub-slice 84 encapsulation).
/// </summary>
public static class BuyListSeedApplyIncomingPacketServices
{
    public static void Queue(Action action) => MerchantApplyIncomingPacketServices.Queue(action);

    public static void ApplyBuyListSeed(BuyListSeedDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            MerchantApplyIncomingPacketServices.Dealer.SetWindowName("Estate");
            MerchantApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Sale");
            MerchantApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Purchase");
            MerchantApplyIncomingPacketServices.Dealer.SetProductType(ProductType.BUY_SEED);
            MerchantApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.List, true, packet.ManorId);
            MerchantApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            MerchantApplyIncomingPacketServices.Dealer.ShowWindow();
        });
}
