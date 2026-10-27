[IncomingGamePacket(GameServerPacketType.CharTemplate)]
public sealed class CharTemplateIncoming : IncomingWirePacket<CharTemplatesDto>
{
    public override void Apply(CharTemplatesDto templates)
    {
        CharacterIncomingPacketServices.ApplyCharTemplate(templates);
    }
}
