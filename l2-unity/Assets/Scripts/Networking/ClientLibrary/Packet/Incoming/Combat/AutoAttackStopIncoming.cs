[IncomingGamePacket(GameServerPacketType.AutoAttackStop)]
public sealed class AutoAttackStopIncoming : IncomingWirePacket<AutoAttackStopDto>
{
    public override void Apply(AutoAttackStopDto packet) =>
        AutoAttackStopIncomingPacketServices.ApplyAutoAttackStop(packet);
}
