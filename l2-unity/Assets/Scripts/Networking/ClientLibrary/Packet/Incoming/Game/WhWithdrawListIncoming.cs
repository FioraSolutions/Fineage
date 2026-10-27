using UnityEngine;

[IncomingGamePacket(GameServerPacketType.WhWithdrawList)]
public sealed class WhWithdrawListIncoming : IncomingWirePacket<WarehouseWithdrawListDto>
{
    public override void Apply(WarehouseWithdrawListDto packet)
    {
        GameIncomingPacketServices.Queue(() =>
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
}
