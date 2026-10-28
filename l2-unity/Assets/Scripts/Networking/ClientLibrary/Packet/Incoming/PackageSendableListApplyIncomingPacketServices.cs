using System;

/// <summary>
/// Package sendable (parcel shipping) list wire packet handler entry (#142 batch slice 148).
/// Splits parcel sendable list apply out of <see cref="MerchantListApplyIncomingPacketServices"/> (sub-slice 84 encapsulation).
/// </summary>
public static class PackageSendableListApplyIncomingPacketServices
{
    public static void Queue(Action action) => MerchantListApplyIncomingPacketServices.Queue(action);

    public static void ApplyPackageSendableList(PackageSendableListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            MerchantListApplyIncomingPacketServices.Dealer.SetWindowName("Send a parcel");
            MerchantListApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            MerchantListApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Shipping list");
            MerchantListApplyIncomingPacketServices.Dealer.SetProductType(ProductType.PackageSendableList);
            MerchantListApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Items, true, packet.PlayerObject);
            MerchantListApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            MerchantListApplyIncomingPacketServices.Dealer.ShowWindow();
        });
}
