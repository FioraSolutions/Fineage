using System;

/// <summary>
/// Buy-list seed (manor estate) wire packet handler entry (#142 batch slice 148).
/// Splits dealer buy-list seed apply out of <see cref="MerchantListApplyIncomingPacketServices"/> (sub-slice 84 encapsulation).
/// </summary>
public static class BuyListSeedApplyIncomingPacketServices
{
    public static void Queue(Action action) => MerchantListApplyIncomingPacketServices.Queue(action);

    public static void ApplyBuyListSeed(BuyListSeedDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            MerchantListApplyIncomingPacketServices.Dealer.SetWindowName("Estate");
            MerchantListApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Sale");
            MerchantListApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Purchase");
            MerchantListApplyIncomingPacketServices.Dealer.SetProductType(ProductType.BUY_SEED);
            MerchantListApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.List, true, packet.ManorId);
            MerchantListApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            MerchantListApplyIncomingPacketServices.Dealer.ShowWindow();
        });
}
