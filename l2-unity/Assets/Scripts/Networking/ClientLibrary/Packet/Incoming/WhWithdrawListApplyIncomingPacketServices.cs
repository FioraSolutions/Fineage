using System;

/// <summary>
/// Personal warehouse withdraw-list wire packet handler entry (#142 batch slice 148).
/// Splits WH withdraw-list apply out of <see cref="GameApplyIncomingPacketServices"/> (sub-slice 83 encapsulation).
/// </summary>
public static class WhWithdrawListApplyIncomingPacketServices
{
    public static void Queue(Action action) => GameApplyIncomingPacketServices.Queue(action);

    public static void ApplyWhWithdrawList(WarehouseWithdrawListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameApplyIncomingPacketServices.Dealer.SetWindowName("Personal storage");
            GameApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Items in Warehouse");
            GameApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Items taken away");
            GameApplyIncomingPacketServices.Dealer.SetProductType(ProductType.WHWithdrawList);
            GameApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, true, -1);
            GameApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameApplyIncomingPacketServices.Dealer.ShowWindow();
        });
}
