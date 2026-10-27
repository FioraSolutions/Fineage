using System;

/// <summary>
/// Merchant / dealer shop wire packet handler access via GameIncomingPacketServices (#142 sub-slice 61).
/// </summary>
public static class MerchantIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplyBuyList(BuyListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameIncomingPacketServices.Dealer.SetWindowName("Shop");
            GameIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Sell");
            GameIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Buy");
            GameIncomingPacketServices.Dealer.SetProductType(ProductType.BUY);
            GameIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, false, packet.ListID);
            GameIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameIncomingPacketServices.Dealer.ShowWindowToCenter();
        });

    public static void ApplyBuyListSeed(BuyListSeedDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameIncomingPacketServices.Dealer.SetWindowName("Estate");
            GameIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Sale");
            GameIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Purchase");
            GameIncomingPacketServices.Dealer.SetProductType(ProductType.BUY_SEED);
            GameIncomingPacketServices.Dealer.UpdateBuyData(packet.List, true, packet.ManorId);
            GameIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameIncomingPacketServices.Dealer.ShowWindow();
        });

    public static void ApplySellList(SellListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameIncomingPacketServices.Dealer.SetWindowName("Shop");
            GameIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            GameIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Sell");
            GameIncomingPacketServices.Dealer.SetProductType(ProductType.SELL);
            GameIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, true, packet.ListID);
            GameIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameIncomingPacketServices.Dealer.ShowWindow();
        });

    public static void ApplyShopPreviewList(ShopPreviewListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Attempt");
            GameIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Selection list");
            GameIncomingPacketServices.Dealer.SetProductType(ProductType.WEAR);
            GameIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, false, packet.ListID);
            GameIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameIncomingPacketServices.Dealer.ShowWindowToCenter();
        });

    public static void ApplyWhDepositList(WarehouseDepositListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameIncomingPacketServices.Dealer.SetWindowName("Personal storage");
            GameIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            GameIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Items in stock");
            GameIncomingPacketServices.Dealer.SetProductType(ProductType.WHDepositList);
            GameIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, true, -1);
            GameIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameIncomingPacketServices.Dealer.ShowWindow();
        });

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

    public static void ApplyPackageSendableList(PackageSendableListDto packet) =>
        Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameIncomingPacketServices.Dealer.SetWindowName("Send a parcel");
            GameIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            GameIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Shipping list");
            GameIncomingPacketServices.Dealer.SetProductType(ProductType.PackageSendableList);
            GameIncomingPacketServices.Dealer.UpdateBuyData(packet.Items, true, packet.PlayerObject);
            GameIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameIncomingPacketServices.Dealer.ShowWindow();
        });
}
