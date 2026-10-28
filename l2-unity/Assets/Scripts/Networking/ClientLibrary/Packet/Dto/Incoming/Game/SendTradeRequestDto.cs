
public class SendTradeRequestDto : IWireDto
{

    private int _senderId;

    public int SenderId { get { return _senderId; } }

    /// <summary>Build from Zone V2 <see cref="ZoneV2MsgType.TradeAsk"/> (1073).</summary>
    public static SendTradeRequestDto FromZoneV2(int requestorObjectId, string requestorName)
    {
        var dto = new SendTradeRequestDto();
        dto._senderId = requestorObjectId;
        return dto;
    }

    

    public void ReadFrom(PacketReader reader)
    {
        _senderId = reader.ReadI();
    }
}
