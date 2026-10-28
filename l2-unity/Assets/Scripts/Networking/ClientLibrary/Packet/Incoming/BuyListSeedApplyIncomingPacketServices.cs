using System;

/// <summary>
/// Buy-list seed (manor estate) wire packet handler entry (#142 batch slice 148).
/// Splits dealer buy-list seed apply out of merchant list hub (sub-slice 84 encapsulation).
/// </summary>
public static class BuyListSeedApplyIncomingPacketServices
{
    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyBuyListSeed(BuyListSeedDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            LocatorApplyIncomingPacketServices.Dealer.SetWindowName("Estate");
            LocatorApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Sale");
            LocatorApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Purchase");
            LocatorApplyIncomingPacketServices.Dealer.SetProductType(ProductType.BUY_SEED);
            LocatorApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.List, true, packet.ManorId);
            LocatorApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            LocatorApplyIncomingPacketServices.Dealer.ShowWindow();
        });
}
