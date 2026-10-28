using System;

/// <summary>
/// Package sendable (parcel shipping) list wire packet handler entry (#142 sub-slice 117).
/// Splits parcel sendable list apply out of <see cref="ShopListIncomingPacketServices"/> (sub-slice 84 encapsulation).
/// </summary>
public static class PackageSendableListIncomingPacketServices
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
