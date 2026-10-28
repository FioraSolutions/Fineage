using System;

/// <summary>
/// Buy-list seed (manor estate) wire packet handler entry (#142 batch slice 148; locator queue slice 260).
/// Splits dealer buy-list seed apply out of merchant list hub (sub-slice 84 encapsulation).
/// </summary>
public static class BuyListSeedApplyIncomingPacketServices
{
    public static DealerWindow Dealer => LocatorApplyIncomingPacketServices.Dealer;

    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyBuyListSeed(BuyListSeedDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            Dealer.SetWindowName("Estate");
            Dealer.SetHeaderNameSellPanel("Sale");
            Dealer.SetHeaderNameBuyPanel("Purchase");
            Dealer.SetProductType(ProductType.BUY_SEED);
            Dealer.UpdateBuyData(packet.List, true, packet.ManorId);
            Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            Dealer.ShowWindow();
        });
}
