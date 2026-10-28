
[IncomingGamePacket(GameServerPacketType.TradeStart)]
public sealed class TradeStartIncoming : IncomingWirePacket<TradeStartDto>
{
    public override void Apply(TradeStartDto packet) =>
        TradeStartApplyIncomingPacketServices.ApplyTradeStart(packet);
}
