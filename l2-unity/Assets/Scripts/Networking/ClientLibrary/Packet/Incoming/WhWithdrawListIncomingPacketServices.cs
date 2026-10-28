using System;

/// <summary>
/// Personal warehouse withdraw-list wire packet handler entry (#142 sub-slice 121).
/// Splits WH withdraw-list apply out of <see cref="GameIncomingPacketServices"/> (sub-slice 83 encapsulation).
/// </summary>
public static class WhWithdrawListIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplyWhWithdrawList(WarehouseWithdrawListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameIncomingPacketServices.Dealer.SetWindowName("Personal storage");
            GameIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Items in Warehouse");
            GameIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Items taken away");
            GameIncomingPacketServices.Dealer.SetProductType(ProductType.WHWithdrawList);
            GameIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, true, -1);
            GameIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameIncomingPacketServices.Dealer.ShowWindow();
        });
}
