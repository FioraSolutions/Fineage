
[IncomingGamePacket(GameServerPacketType.MultiSellList)]
public sealed class MultiSellListIncoming : IncomingWirePacket<MultiSellListDto>
{
    public override void Apply(MultiSellListDto packet) => MultiSellListIncomingPacketServices.ApplyMultiSellList(packet);
}
