
[IncomingGamePacket(GameServerPacketType.SendTradeRequest)]
public sealed class SendTradeRequestIncoming : IncomingWirePacket<SendTradeRequestDto>
{
    public override void Apply(SendTradeRequestDto packet) =>
        TradeOpenIncomingPacketServices.ApplySendTradeRequest(packet);
}
