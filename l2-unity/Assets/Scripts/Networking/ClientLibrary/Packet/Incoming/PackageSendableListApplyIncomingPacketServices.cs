using System;

/// <summary>
/// Package sendable (parcel shipping) list wire packet handler entry (#142 batch slice 148).
/// Splits parcel sendable list apply out of <see cref="MerchantListApplyIncomingPacketServices"/> (sub-slice 84 encapsulation).
/// </summary>
public static class PackageSendableListApplyIncomingPacketServices
{
    public static void Queue(Action action) => GameApplyIncomingPacketServices.Queue(action);

    public static void ApplyPackageSendableList(PackageSendableListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameApplyIncomingPacketServices.Dealer.SetWindowName("Send a parcel");
            GameApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            GameApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Shipping list");
            GameApplyIncomingPacketServices.Dealer.SetProductType(ProductType.PackageSendableList);
            GameApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Items, true, packet.PlayerObject);
            GameApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameApplyIncomingPacketServices.Dealer.ShowWindow();
        });
}
