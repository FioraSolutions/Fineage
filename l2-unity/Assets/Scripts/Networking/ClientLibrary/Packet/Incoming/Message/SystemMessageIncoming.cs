[IncomingGamePacket(GameServerPacketType.SystemMessage)]
public sealed class SystemMessageIncoming : IncomingWirePacket<SystemMessageDto>
{
    public override void Apply(SystemMessageDto packet) =>
        SystemMessageIncomingPacketServices.ApplySystemMessage(packet);
}
