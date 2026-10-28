using System;

/// <summary>
/// Personal warehouse withdraw-list wire packet handler entry (#142 batch slice 148).
/// Splits WH withdraw-list apply out of <see cref="GameApplyIncomingPacketServices"/> (sub-slice 83 encapsulation).
/// </summary>
public static class WhWithdrawListApplyIncomingPacketServices
{
    public static void Queue(Action action) => MerchantApplyIncomingPacketServices.Queue(action);

    public static void ApplyWhWithdrawList(WarehouseWithdrawListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            MerchantApplyIncomingPacketServices.Dealer.SetWindowName("Personal storage");
            MerchantApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Items in Warehouse");
            MerchantApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Items taken away");
            MerchantApplyIncomingPacketServices.Dealer.SetProductType(ProductType.WHWithdrawList);
            MerchantApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, true, -1);
            MerchantApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            MerchantApplyIncomingPacketServices.Dealer.ShowWindow();
        });
}
