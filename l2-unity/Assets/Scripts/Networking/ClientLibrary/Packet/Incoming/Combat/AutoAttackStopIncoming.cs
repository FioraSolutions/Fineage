[IncomingGamePacket(GameServerPacketType.AutoAttackStop)]
public sealed class AutoAttackStopIncoming : IncomingWirePacket<AutoAttackStopDto>
{
    public override void Apply(AutoAttackStopDto packet)
    {
        CombatIncomingPacketServices.ApplyWorld(apply => apply.AutoAttackStop(packet));
    }
}
