using UnityEngine;

[IncomingGamePacket(GameServerPacketType.NpcHtmlMessage)]
public sealed class NpcHtmlMessageIncoming : IncomingWirePacket<NpcHtmlMessageDto>
{
    public override void Apply(NpcHtmlMessageDto packet)
    {
        IncomingPacketServices.QueueWorld(apply => apply.ShowNpcHtml(packet));
    }
}
