using System;

/// <summary>
/// Package sendable (parcel shipping) list wire packet handler entry (#142 batch slice 148).
/// Splits parcel sendable list apply out of merchant list hub (sub-slice 84 encapsulation).
/// </summary>
public static class PackageSendableListApplyIncomingPacketServices
{
    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyPackageSendableList(PackageSendableListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            LocatorApplyIncomingPacketServices.Dealer.SetWindowName("Send a parcel");
            LocatorApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            LocatorApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Shipping list");
            LocatorApplyIncomingPacketServices.Dealer.SetProductType(ProductType.PackageSendableList);
            LocatorApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Items, true, packet.PlayerObject);
            LocatorApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            LocatorApplyIncomingPacketServices.Dealer.ShowWindow();
        });
}
