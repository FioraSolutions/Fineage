using UnityEngine;

[IncomingGamePacket(GameServerPacketType.SellList)]
public sealed class SellListIncoming : IncomingWirePacket<SellListDto>
{
    public override void Apply(SellListDto packet)
    {
        GameIncomingPacketServices.Queue(() =>
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
    }
}
