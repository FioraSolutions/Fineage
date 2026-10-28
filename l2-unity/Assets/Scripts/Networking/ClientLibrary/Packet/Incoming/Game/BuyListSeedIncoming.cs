
[IncomingGamePacket(GameServerPacketType.BuyListSeed)]
public sealed class BuyListSeedIncoming : IncomingWirePacket<BuyListSeedDto>
{
    public override void Apply(BuyListSeedDto packet) =>
        ShopListIncomingPacketServices.ApplyBuyListSeed(packet);
}
