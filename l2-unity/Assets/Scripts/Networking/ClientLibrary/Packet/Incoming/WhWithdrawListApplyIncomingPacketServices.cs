using System;

/// <summary>
/// Personal warehouse withdraw-list wire packet handler entry (#142 batch slice 148).
/// Splits WH withdraw-list apply out of <see cref="MerchantListApplyIncomingPacketServices"/> (sub-slice 83 encapsulation).
/// </summary>
public static class WhWithdrawListApplyIncomingPacketServices
{
    public static void Queue(Action action) => MerchantListApplyIncomingPacketServices.Queue(action);

    public static void ApplyWhWithdrawList(WarehouseWithdrawListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            MerchantListApplyIncomingPacketServices.Dealer.SetWindowName("Personal storage");
            MerchantListApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Items in Warehouse");
            MerchantListApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Items taken away");
            MerchantListApplyIncomingPacketServices.Dealer.SetProductType(ProductType.WHWithdrawList);
            MerchantListApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, true, -1);
            MerchantListApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            MerchantListApplyIncomingPacketServices.Dealer.ShowWindow();
        });
}
