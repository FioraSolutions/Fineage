[IncomingGamePacket(GameServerPacketType.AutoAttackStart)]
public sealed class AutoAttackStartIncoming : IncomingWirePacket<AutoAttackStartDto>
{
    public override void Apply(AutoAttackStartDto packet)
    {
        IncomingPacketServices.ApplyWorld(apply => apply.AutoAttackStart(packet));
    }
}
