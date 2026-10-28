[IncomingGamePacket(GameServerPacketType.ShopPreviewInfo)]
public sealed class ShopPreviewInfoIncoming : IncomingWirePacket<ShopPreviewInfoDto>
{
    public override void Apply(ShopPreviewInfoDto packet) =>
        ShopPreviewApplyIncomingPacketServices.ApplyShopPreviewInfo(packet);
}
