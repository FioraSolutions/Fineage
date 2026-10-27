using UnityEngine;

[IncomingGamePacket(GameServerPacketType.ShopPreviewList)]
public sealed class ShopPreviewListIncoming : IncomingWirePacket<ShopPreviewListDto>
{
    public override void Apply(ShopPreviewListDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            IncomingPacketServices.Dealer.SetHeaderNameSellPanel("Attempt");
            IncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Selection list");
            IncomingPacketServices.Dealer.SetProductType(ProductType.WEAR);
            IncomingPacketServices.Dealer.UpdateBuyData(packet.Products, false, packet.ListID);
            IncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            IncomingPacketServices.Dealer.ShowWindowToCenter();
        });
    }
}
