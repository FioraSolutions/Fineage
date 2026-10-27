using UnityEngine;

[IncomingGamePacket(GameServerPacketType.ShopPreviewInfo)]
public sealed class ShopPreviewInfoIncoming : IncomingWirePacket<ShopPreviewInfoDto>
{
    public override void Apply(ShopPreviewInfoDto packet)
    {
        StubIncomingPacketServices.ApplyShopPreviewInfo(packet);
    }
}
