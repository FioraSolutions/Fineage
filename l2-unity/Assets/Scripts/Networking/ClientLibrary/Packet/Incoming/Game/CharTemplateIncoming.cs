[IncomingGamePacket(GameServerPacketType.CharTemplate)]
public sealed class CharTemplateIncoming : IncomingWirePacket<CharTemplatesDto>
{
    public override void Apply(CharTemplatesDto templates) =>
        CharTemplateApplyIncomingPacketServices.ApplyCharTemplate(templates);
}
