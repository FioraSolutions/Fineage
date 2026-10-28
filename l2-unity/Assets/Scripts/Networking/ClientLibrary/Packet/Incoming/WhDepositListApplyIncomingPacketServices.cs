using System;

/// <summary>
/// Personal warehouse deposit-list wire packet handler entry (#142 batch slice 148).
/// Splits WH deposit-list apply out of <see cref="MerchantListApplyIncomingPacketServices"/> (sub-slice 83 encapsulation).
/// </summary>
public static class WhDepositListApplyIncomingPacketServices
{
    public static void Queue(Action action) => GameApplyIncomingPacketServices.Queue(action);

    public static void ApplyWhDepositList(WarehouseDepositListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameApplyIncomingPacketServices.Dealer.SetWindowName("Personal storage");
            GameApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            GameApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Items in stock");
            GameApplyIncomingPacketServices.Dealer.SetProductType(ProductType.WHDepositList);
            GameApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, true, -1);
            GameApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameApplyIncomingPacketServices.Dealer.ShowWindow();
        });
}
