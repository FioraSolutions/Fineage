using UnityEngine;

[IncomingGamePacket(GameServerPacketType.BuyList)]
public sealed class BuyListIncoming : IncomingWirePacket<BuyListDto>
{
    public override void Apply(BuyListDto packet)
    {
        GameIncomingPacketServices.Queue(() =>
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
    }
}
