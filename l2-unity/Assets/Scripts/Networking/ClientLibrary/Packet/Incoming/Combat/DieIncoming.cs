[IncomingGamePacket(GameServerPacketType.Die)]
public sealed class DieIncoming : IncomingWirePacket<DieDto>
{
    public override void Apply(DieDto packet) =>
        DieIncomingPacketServices.ApplyDie(packet);
}
