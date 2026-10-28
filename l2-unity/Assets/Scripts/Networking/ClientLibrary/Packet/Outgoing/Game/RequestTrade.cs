[OutgoingCommandPacket(typeof(RequestTradeCommand))]
public sealed class RequestTrade : OutgoingWirePacket<RequestTradeDto>
{
    protected override byte Opcode => (byte)GameClientPacketType.TradeRequest;

    public RequestTrade(RequestTradeCommand command) : this(command.TargetObjectId) { }

    public RequestTrade(int targetObjectId)
    {
        Dto.TargetObjectId = targetObjectId;
    }
}
