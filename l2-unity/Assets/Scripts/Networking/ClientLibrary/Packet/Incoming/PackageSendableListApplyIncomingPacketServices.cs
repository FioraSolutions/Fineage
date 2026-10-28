using System;

/// <summary>
/// Package sendable (parcel shipping) list wire packet handler entry (#142 batch slice 148).
/// Splits parcel sendable list apply out of <see cref="GameApplyIncomingPacketServices"/> (sub-slice 84 encapsulation).
/// </summary>
public static class PackageSendableListApplyIncomingPacketServices
{
    public static void Queue(Action action) => MerchantApplyIncomingPacketServices.Queue(action);

    public static void ApplyPackageSendableList(PackageSendableListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            MerchantApplyIncomingPacketServices.Dealer.SetWindowName("Send a parcel");
            MerchantApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            MerchantApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Shipping list");
            MerchantApplyIncomingPacketServices.Dealer.SetProductType(ProductType.PackageSendableList);
            MerchantApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Items, true, packet.PlayerObject);
            MerchantApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            MerchantApplyIncomingPacketServices.Dealer.ShowWindow();
        });
}
