
public class SendTradeRequestDto : IWireDto
{

    private int _senderId;

    public int SenderId { get { return _senderId; } }

    /// <summary>Build from the Fineage Net trade ask row (1073).</summary>
    public static SendTradeRequestDto FromNet(int requestorObjectId, string requestorName)
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
