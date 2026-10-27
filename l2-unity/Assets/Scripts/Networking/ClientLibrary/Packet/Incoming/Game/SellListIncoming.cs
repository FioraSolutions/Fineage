using UnityEngine;

[IncomingGamePacket(GameServerPacketType.SellList)]
public sealed class SellListIncoming : IncomingWirePacket<SellListDto>
{
    public override void Apply(SellListDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            IncomingPacketServices.Dealer.SetWindowName("Shop");
            IncomingPacketServices.Dealer.SetHeaderNameSellPanel("Inventory");
            IncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Sell");
            IncomingPacketServices.Dealer.SetProductType(ProductType.SELL);
            IncomingPacketServices.Dealer.UpdateBuyData(packet.Products, true, packet.ListID);
            IncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            IncomingPacketServices.Dealer.ShowWindow();
        });
    }
}
