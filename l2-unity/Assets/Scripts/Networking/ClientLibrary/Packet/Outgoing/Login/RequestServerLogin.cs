[OutgoingCommandPacket(typeof(RequestServerLoginCommand))]
public sealed class RequestServerLogin : OutgoingWirePacket<RequestServerLoginDto>
{
    protected override byte Opcode => (byte)LoginClientPacketType.RequestServerLogin;

    // Must match other login C→S packets (checksum + pad), not game framing.
    protected override OutgoingBuildKind BuildKind => OutgoingBuildKind.Login;
    protected override int LoginExtraZeroBytes => 0;

    public RequestServerLogin(RequestServerLoginCommand command) : this(command.ServerId, command.SessionKey1, command.SessionKey2) { }

    public RequestServerLogin(int serverId, int sessionKey1, int sessionKey2)
    {
        Dto.ServerId = serverId;
        Dto.SessionKey1 = sessionKey1;
        Dto.SessionKey2 = sessionKey2;
    }
}
