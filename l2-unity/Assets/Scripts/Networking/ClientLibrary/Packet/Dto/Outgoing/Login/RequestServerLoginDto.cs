public sealed class RequestServerLoginDto : IOutgoingDto
{
    public int ServerId;
    public int SessionKey1;
    public int SessionKey2;

    public void WriteTo(PacketWriter writer)
    {
        // Interlude RequestServerLogin: D sessionKey1, D sessionKey2, C serverId
        writer.WriteI(SessionKey1);
        writer.WriteI(SessionKey2);
        writer.WriteB((byte)ServerId);
    }
}
