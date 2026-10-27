using UnityEngine;

[IncomingGamePacket(GameServerPacketType.BuyList)]
public sealed class BuyListIncoming : IncomingWirePacket<BuyListDto>
{
    public override void Apply(BuyListDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            IncomingPacketServices.Dealer.SetWindowName("Shop");
            IncomingPacketServices.Dealer.SetHeaderNameSellPanel("Sell");
            IncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Buy");
            IncomingPacketServices.Dealer.SetProductType(ProductType.BUY);
            IncomingPacketServices.Dealer.UpdateBuyData(packet.Products, false, packet.ListID);
            IncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            IncomingPacketServices.Dealer.ShowWindowToCenter();
        });
    }
}
