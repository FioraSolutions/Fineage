using System;

/// <summary>
/// Dealer shop list wire packet handler access via GameIncomingPacketServices (#142 sub-slice 84).
/// Complements warehouse list handlers on <see cref="WarehouseIncomingPacketServices"/> (sub-slice 83),
/// unwired shop preview info on <see cref="ShopPreviewIncomingPacketServices"/> (sub-slice 98),
/// sell list on <see cref="SellListIncomingPacketServices"/> (sub-slice 113),
/// buy list on <see cref="BuyListIncomingPacketServices"/> (sub-slice 114),
/// buy-list seed (manor estate) on <see cref="BuyListSeedIncomingPacketServices"/> (sub-slice 115),
/// and shop preview list on <see cref="ShopPreviewListIncomingPacketServices"/> (sub-slice 116).
/// </summary>
public static class ShopListIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplyPackageSendableList(PackageSendableListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameIncomingPacketServices.Dealer.SetWindowName("Send a parcel");
            GameIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            GameIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Shipping list");
            GameIncomingPacketServices.Dealer.SetProductType(ProductType.PackageSendableList);
            GameIncomingPacketServices.Dealer.UpdateBuyData(packet.Items, true, packet.PlayerObject);
            GameIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameIncomingPacketServices.Dealer.ShowWindow();
        });
}
