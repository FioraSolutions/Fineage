
[IncomingGamePacket(GameServerPacketType.NpcHtmlMessage)]
public sealed class NpcHtmlMessageIncoming : IncomingWirePacket<NpcHtmlMessageDto>
{
    public override void Apply(NpcHtmlMessageDto packet) =>
        NpcIncomingPacketServices.ApplyNpcHtmlMessage(packet);
}
