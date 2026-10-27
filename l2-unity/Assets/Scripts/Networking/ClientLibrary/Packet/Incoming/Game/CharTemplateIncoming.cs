using UnityEngine;

[IncomingGamePacket(GameServerPacketType.CharTemplate)]
public sealed class CharTemplateIncoming : IncomingWirePacket<CharTemplatesDto>
{
    public override void Apply(CharTemplatesDto templates)
    {
        var list = templates.PlayerTemplates;
        CharacterIncomingPacketServices.Queue(() => CharacterIncomingPacketServices.Manager.OnCreateUser(list));
    }
}
