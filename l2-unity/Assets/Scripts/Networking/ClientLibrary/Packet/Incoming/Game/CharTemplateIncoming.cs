using UnityEngine;

[IncomingGamePacket(GameServerPacketType.CharTemplate)]
public sealed class CharTemplateIncoming : IncomingWirePacket<CharTemplatesDto>
{
    public override void Apply(CharTemplatesDto templates)
    {
        var list = templates.PlayerTemplates;
        IncomingPacketServices.Queue(() => IncomingPacketServices.Manager.OnCreateUser(list));
    }
}
