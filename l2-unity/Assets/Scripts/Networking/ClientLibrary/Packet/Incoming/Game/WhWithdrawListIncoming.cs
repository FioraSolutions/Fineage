using UnityEngine;

[IncomingGamePacket(GameServerPacketType.WhWithdrawList)]
public sealed class WhWithdrawListIncoming : IncomingWirePacket<WarehouseWithdrawListDto>
{
    public override void Apply(WarehouseWithdrawListDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            IncomingPacketServices.Dealer.SetWindowName("Personal storage");
            IncomingPacketServices.Dealer.SetHeaderNameSellPanel("Items in Warehouse");
            IncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Items taken away");
            IncomingPacketServices.Dealer.SetProductType(ProductType.WHWithdrawList);
            IncomingPacketServices.Dealer.UpdateBuyData(packet.Products, true, -1);
            IncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            IncomingPacketServices.Dealer.ShowWindow();
        });
    }
}
