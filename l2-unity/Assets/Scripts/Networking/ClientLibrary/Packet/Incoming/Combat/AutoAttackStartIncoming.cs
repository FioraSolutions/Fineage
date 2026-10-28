[IncomingGamePacket(GameServerPacketType.AutoAttackStart)]
public sealed class AutoAttackStartIncoming : IncomingWirePacket<AutoAttackStartDto>
{
    public override void Apply(AutoAttackStartDto packet) =>
        AutoAttackStartIncomingPacketServices.ApplyAutoAttackStart(packet);
}
