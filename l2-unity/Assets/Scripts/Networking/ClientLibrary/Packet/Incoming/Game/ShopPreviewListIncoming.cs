using UnityEngine;

[IncomingGamePacket(GameServerPacketType.ShopPreviewList)]
public sealed class ShopPreviewListIncoming : IncomingWirePacket<ShopPreviewListDto>
{
    public override void Apply(ShopPreviewListDto packet)
    {
        GameIncomingPacketServices.Queue(() =>
        {
            UserInfoDto info = StorageNpc.getInstance().GetFirstUser();
            GameIncomingPacketServices.Dealer.SetHeaderNameSellPanel("Attempt");
            GameIncomingPacketServices.Dealer.SetHeaderNameBuyPanel("Selection list");
            GameIncomingPacketServices.Dealer.SetProductType(ProductType.WEAR);
            GameIncomingPacketServices.Dealer.UpdateBuyData(packet.Products, false, packet.ListID);
            GameIncomingPacketServices.Dealer.UpdateDataForm(packet.CurrentMoney, info.PlayerInfoInterlude.Stats.WeightPercent(), info.PlayerInfoInterlude.Stats.CurrWeight, info.PlayerInfoInterlude.Stats.MaxWeight);
            GameIncomingPacketServices.Dealer.ShowWindowToCenter();
        });
    }
}
