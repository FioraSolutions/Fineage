using System;

/// <summary>
/// Personal warehouse deposit-list wire packet handler entry (#142 batch slice 148).
/// Splits WH deposit-list apply out of merchant list hub (sub-slice 83 encapsulation).
/// </summary>
public static class WhDepositListApplyIncomingPacketServices
{
    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyWhDepositList(WarehouseDepositListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            LocatorApplyIncomingPacketServices.Dealer.SetWindowName("Personal storage");
            LocatorApplyIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            LocatorApplyIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Items in stock");
            LocatorApplyIncomingPacketServices.Dealer.SetProductType(ProductType.WHDepositList);
            LocatorApplyIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, true, -1);
            LocatorApplyIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            LocatorApplyIncomingPacketServices.Dealer.ShowWindow();
        });
}
