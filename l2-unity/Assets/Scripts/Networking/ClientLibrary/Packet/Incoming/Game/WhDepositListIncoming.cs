using UnityEngine;

[IncomingGamePacket(GameServerPacketType.WhDepositList)]
public sealed class WhDepositListIncoming : IncomingWirePacket<WarehouseDepositListDto>
{
    public override void Apply(WarehouseDepositListDto packet)
    {
        GameIncomingPacketServices.Queue(() =>
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
    }
}
