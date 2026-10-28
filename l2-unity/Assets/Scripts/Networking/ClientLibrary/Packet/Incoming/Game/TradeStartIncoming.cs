
[IncomingGamePacket(GameServerPacketType.TradeStart)]
public sealed class TradeStartIncoming : IncomingWirePacket<TradeStartDto>
{
    public override void Apply(TradeStartDto packet) =>
        TradeStartIncomingPacketServices.ApplyTradeStart(packet);
}
