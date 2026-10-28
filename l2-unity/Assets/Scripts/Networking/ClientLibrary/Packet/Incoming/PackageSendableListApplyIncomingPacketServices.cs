using System;

/// <summary>
/// Package sendable (parcel shipping) list wire packet handler entry (#142 batch slice 148; locator queue slice 260).
/// Splits parcel sendable list apply out of merchant list hub (sub-slice 84 encapsulation).
/// </summary>
public static class PackageSendableListApplyIncomingPacketServices
{
    public static DealerWindow Dealer => LocatorApplyIncomingPacketServices.Dealer;

    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyPackageSendableList(PackageSendableListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            Dealer.SetWindowName("Send a parcel");
            Dealer.SetHeaderNameSellPanel("Inventory");
            Dealer.SetHeaderNameBuyPanel("Shipping list");
            Dealer.SetProductType(ProductType.PackageSendableList);
            Dealer.UpdateBuyData(packet.Items, true, packet.PlayerObject);
            Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            Dealer.ShowWindow();
        });
}
