using System;

/// <summary>
/// Personal warehouse deposit-list wire packet handler entry (#142 batch slice 148; locator queue slice 260).
/// Splits WH deposit-list apply out of merchant list hub (sub-slice 83 encapsulation).
/// </summary>
public static class WhDepositListApplyIncomingPacketServices
{
    public static DealerWindow Dealer => LocatorApplyIncomingPacketServices.Dealer;

    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyWhDepositList(WarehouseDepositListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            Dealer.SetWindowName("Personal storage");
            Dealer.SetHeaderNameSellPanel("Inventory");
            Dealer.SetHeaderNameBuyPanel("Items in stock");
            Dealer.SetProductType(ProductType.WHDepositList);
            Dealer.UpdateBuyData(packet.Products, true, -1);
            Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            Dealer.ShowWindow();
        });
}
