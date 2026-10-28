using System;

/// <summary>
/// Personal warehouse deposit-list wire packet handler entry (#142 batch slice 148).
/// Splits WH deposit-list apply out of <see cref="GameApplyIncomingPacketServices"/> (sub-slice 83 encapsulation).
/// </summary>
public static class WhDepositListApplyIncomingPacketServices
{
    public static void Queue(Action action) => MerchantApplyIncomingPacketServices.Queue(action);

    public static void ApplyWhDepositList(WarehouseDepositListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            MerchantApplyIncomingPacketServices.Dealer.SetWindowName("Personal storage");
            MerchantApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            MerchantApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Items in stock");
            MerchantApplyIncomingPacketServices.Dealer.SetProductType(ProductType.WHDepositList);
            MerchantApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, true, -1);
            MerchantApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            MerchantApplyIncomingPacketServices.Dealer.ShowWindow();
        });
}
