using UnityEngine;

[IncomingGamePacket(GameServerPacketType.Attack)]
public sealed class AttackIncoming : IncomingWirePacket<AttackDto>
{
    public override void Apply(AttackDto packet)
    {
        IncomingPacketServices.ApplyWorld(apply => apply.Attack(packet));
    }
}
