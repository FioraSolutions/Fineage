[IncomingGamePacket(GameServerPacketType.AutoAttackStart)]
public sealed class AutoAttackStartIncoming : IncomingWirePacket<AutoAttackStartDto>
{
    public override void Apply(AutoAttackStartDto packet)
    {
        CombatIncomingPacketServices.ApplyWorld(apply => apply.AutoAttackStart(packet));
    }
}
