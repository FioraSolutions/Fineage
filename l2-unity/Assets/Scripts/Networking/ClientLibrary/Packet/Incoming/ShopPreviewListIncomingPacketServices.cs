using System;

/// <summary>
/// Shop preview list wire packet handler entry (#142 sub-slice 116).
/// Splits preview-wear list apply out of <see cref="GameIncomingPacketServices"/> (sub-slice 84 encapsulation).
/// Complements unwired preview info on <see cref="ShopPreviewIncomingPacketServices"/> (sub-slice 98).
/// </summary>
public static class ShopPreviewListIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplyShopPreviewList(ShopPreviewListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Attempt");
            GameIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Selection list");
            GameIncomingPacketServices.Dealer.SetProductType(ProductType.WEAR);
            GameIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, false, packet.ListID);
            GameIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameIncomingPacketServices.Dealer.ShowWindowToCenter();
        });
}
