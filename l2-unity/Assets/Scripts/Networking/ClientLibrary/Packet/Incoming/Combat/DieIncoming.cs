[IncomingGamePacket(GameServerPacketType.Die)]
public sealed class DieIncoming : IncomingWirePacket<DieDto>
{
    public override void Apply(DieDto packet)
    {
        CombatIncomingPacketServices.ApplyWorld(apply => apply.Die(packet));
    }
}
