[IncomingGamePacket(GameServerPacketType.NpcSay)]
public sealed class NpcSayIncoming : IncomingWirePacket<NpcSayDto>
{
    public override void Apply(NpcSayDto packet) =>
        NpcSayApplyIncomingPacketServices.ApplyNpcSay(packet);
}
