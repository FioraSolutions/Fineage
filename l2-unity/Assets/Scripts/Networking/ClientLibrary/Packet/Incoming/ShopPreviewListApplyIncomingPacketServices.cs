using System;

/// <summary>
/// Shop preview list wire packet handler entry (#142 batch slice 148).
/// Splits preview-wear list apply out of <see cref="MerchantListApplyIncomingPacketServices"/> (sub-slice 84 encapsulation).
/// Complements unwired preview info on <see cref="ShopPreviewApplyIncomingPacketServices"/> (sub-slice 98).
/// </summary>
public static class ShopPreviewListApplyIncomingPacketServices
{
    public static void Queue(Action action) => MerchantListApplyIncomingPacketServices.Queue(action);

    public static void ApplyShopPreviewList(ShopPreviewListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            MerchantListApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Attempt");
            MerchantListApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Selection list");
            MerchantListApplyIncomingPacketServices.Dealer.SetProductType(ProductType.WEAR);
            MerchantListApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, false, packet.ListID);
            MerchantListApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            MerchantListApplyIncomingPacketServices.Dealer.ShowWindowToCenter();
        });
}
