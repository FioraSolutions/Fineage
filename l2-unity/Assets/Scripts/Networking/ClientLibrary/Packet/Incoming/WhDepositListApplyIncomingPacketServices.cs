using System;

/// <summary>
/// Personal warehouse deposit-list wire packet handler entry (#142 batch slice 148).
/// Splits WH deposit-list apply out of <see cref="MerchantListApplyIncomingPacketServices"/> (sub-slice 83 encapsulation).
/// </summary>
public static class WhDepositListApplyIncomingPacketServices
{
    public static void Queue(Action action) => MerchantListApplyIncomingPacketServices.Queue(action);

    public static void ApplyWhDepositList(WarehouseDepositListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            MerchantListApplyIncomingPacketServices.Dealer.SetWindowName("Personal storage");
            MerchantListApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            MerchantListApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Items in stock");
            MerchantListApplyIncomingPacketServices.Dealer.SetProductType(ProductType.WHDepositList);
            MerchantListApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, true, -1);
            MerchantListApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            MerchantListApplyIncomingPacketServices.Dealer.ShowWindow();
        });
}
