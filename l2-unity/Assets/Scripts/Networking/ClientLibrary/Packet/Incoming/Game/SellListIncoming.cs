
[IncomingGamePacket(GameServerPacketType.SellList)]
public sealed class SellListIncoming : IncomingWirePacket<SellListDto>
{
    public override void Apply(SellListDto packet) =>
        SellListApplyIncomingPacketServices.ApplySellList(packet);
}
