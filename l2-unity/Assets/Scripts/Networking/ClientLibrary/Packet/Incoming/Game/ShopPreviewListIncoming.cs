
[IncomingGamePacket(GameServerPacketType.ShopPreviewList)]
public sealed class ShopPreviewListIncoming : IncomingWirePacket<ShopPreviewListDto>
{
    public override void Apply(ShopPreviewListDto packet) =>
        ShopPreviewListApplyIncomingPacketServices.ApplyShopPreviewList(packet);
}
