[IncomingGamePacket(GameServerPacketType.NpcSay)]
public sealed class NpcSayIncoming : IncomingWirePacket<NpcSayDto>
{
    public override void Apply(NpcSayDto packet) =>
        NpcSayIncomingPacketServices.ApplyNpcSay(packet);
}
