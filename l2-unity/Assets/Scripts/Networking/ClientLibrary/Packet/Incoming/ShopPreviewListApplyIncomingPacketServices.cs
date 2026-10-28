using System;

/// <summary>
/// Shop preview list wire packet handler entry (#142 batch slice 148).
/// Splits preview-wear list apply out of <see cref="MerchantListApplyIncomingPacketServices"/> (sub-slice 84 encapsulation).
/// Complements unwired preview info on <see cref="ShopPreviewApplyIncomingPacketServices"/> (sub-slice 98).
/// </summary>
public static class ShopPreviewListApplyIncomingPacketServices
{
    public static void Queue(Action action) => GameApplyIncomingPacketServices.Queue(action);

    public static void ApplyShopPreviewList(ShopPreviewListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Attempt");
            GameApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Selection list");
            GameApplyIncomingPacketServices.Dealer.SetProductType(ProductType.WEAR);
            GameApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, false, packet.ListID);
            GameApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameApplyIncomingPacketServices.Dealer.ShowWindowToCenter();
        });
}
