
[IncomingGamePacket(GameServerPacketType.BuyList)]
public sealed class BuyListIncoming : IncomingWirePacket<BuyListDto>
{
    public override void Apply(BuyListDto packet) =>
        BuyListApplyIncomingPacketServices.ApplyBuyList(packet);
}
