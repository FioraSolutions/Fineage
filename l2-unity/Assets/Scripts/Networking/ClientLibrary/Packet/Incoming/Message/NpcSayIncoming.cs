[IncomingGamePacket(GameServerPacketType.NpcSay)]
public sealed class NpcSayIncoming : IncomingWirePacket<NpcSayDto>
{
    public override void Apply(NpcSayDto packet)
    {
        if (packet.NpcMessage == null)
            return;

        MessagePacketServices.ApplyMessage(apply => apply.NpcSay(packet.NpcMessage));
    }
}
