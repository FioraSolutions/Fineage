[IncomingGamePacket(GameServerPacketType.SystemMessage)]
public sealed class SystemMessageIncoming : IncomingWirePacket<SystemMessageDto>
{
    public override void Apply(SystemMessageDto packet) =>
        SystemChatApplyIncomingPacketServices.ApplySystemMessage(packet);
}
