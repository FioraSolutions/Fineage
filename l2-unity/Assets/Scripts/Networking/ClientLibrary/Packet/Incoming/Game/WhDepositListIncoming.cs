using UnityEngine;

[IncomingGamePacket(GameServerPacketType.WhDepositList)]
public sealed class WhDepositListIncoming : IncomingWirePacket<WarehouseDepositListDto>
{
    public override void Apply(WarehouseDepositListDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            IncomingPacketServices.Dealer.SetWindowName("Personal storage");
            IncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            IncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Items in stock");
            IncomingPacketServices.Dealer.SetProductType(ProductType.WHDepositList);
            IncomingPacketServices.Dealer.UpdateBuyData(packet.Products, true, -1);
            IncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            IncomingPacketServices.Dealer.ShowWindow();
        });
    }
}
