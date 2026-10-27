[OutgoingCommandPacket(typeof(ProtocolVersionCommand))]
public sealed class ProtocolVersion : OutgoingWirePacket<ProtocolVersionDto>
{
    protected override byte Opcode => (byte)GameClientPacketType.ProtocolVersion;

    // C + D protocol only. Default GamePad appends a junk zero-int (Unity was sending len=16; smoke uses 8).
    protected override OutgoingBuildKind BuildKind => OutgoingBuildKind.GameNoPad;

    public ProtocolVersion(ProtocolVersionCommand command) : this(command.Protocol) { }

    public ProtocolVersion(int protocol)
    {
        Dto.Protocol = protocol;
    }
}
