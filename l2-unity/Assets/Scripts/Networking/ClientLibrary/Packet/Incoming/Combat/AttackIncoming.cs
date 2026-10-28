[IncomingGamePacket(GameServerPacketType.Attack)]
public sealed class AttackIncoming : IncomingWirePacket<AttackDto>
{
    public override void Apply(AttackDto packet) =>
        AttackApplyIncomingPacketServices.ApplyAttack(packet);
}
