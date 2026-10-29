using System;

/// <summary>
/// Shop preview list wire packet handler entry (#142 batch slice 148; locator queue slice 260).
/// Splits preview-wear list apply out of merchant list hub (sub-slice 84 encapsulation).
/// Complements unwired preview info on <see cref="ShopPreviewApplyIncomingPacketServices"/> (sub-slice 98).
/// </summary>
public static class ShopPreviewListApplyIncomingPacketServices
{
    public static DealerWindow Dealer => LocatorApplyIncomingPacketServices.Dealer;

    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyShopPreviewList(ShopPreviewListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            Dealer.SetHeaderNameSellPanel("Attempt");
            Dealer.SetHeaderNameBuyPanel("Selection list");
            Dealer.SetProductType(ProductType.WEAR);
            Dealer.UpdateBuyData(packet.Products, false, packet.ListID);
            Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            Dealer.ShowWindowToCenter();
        });
}
