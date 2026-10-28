[IncomingGamePacket(GameServerPacketType.CreatureSay)]
public sealed class CreatureSayIncoming : IncomingWirePacket<CreatureSayDto>
{
    public override void Apply(CreatureSayDto packet) =>
        CreatureSayApplyIncomingPacketServices.ApplyCreatureSay(packet);
}
