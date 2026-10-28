using System;

/// <summary>
/// Shop preview list wire packet handler entry (#142 batch slice 148).
/// Splits preview-wear list apply out of merchant list hub (sub-slice 84 encapsulation).
/// Complements unwired preview info on <see cref="ShopPreviewApplyIncomingPacketServices"/> (sub-slice 98).
/// </summary>
public static class ShopPreviewListApplyIncomingPacketServices
{
    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyShopPreviewList(ShopPreviewListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            LocatorApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Attempt");
            LocatorApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Selection list");
            LocatorApplyIncomingPacketServices.Dealer.SetProductType(ProductType.WEAR);
            LocatorApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, false, packet.ListID);
            LocatorApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            LocatorApplyIncomingPacketServices.Dealer.ShowWindowToCenter();
        });
}
