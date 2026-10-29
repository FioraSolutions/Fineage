public sealed class RequestTradeDto : IOutgoingDto
{
    public int TargetObjectId;

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteI(TargetObjectId);
    }
}
