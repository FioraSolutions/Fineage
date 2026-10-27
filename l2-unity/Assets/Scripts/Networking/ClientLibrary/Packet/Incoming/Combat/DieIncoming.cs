[IncomingGamePacket(GameServerPacketType.Die)]
public sealed class DieIncoming : IncomingWirePacket<DieDto>
{
    public override void Apply(DieDto packet)
    {
        IncomingPacketServices.ApplyWorld(apply => apply.Die(packet));
    }
}
