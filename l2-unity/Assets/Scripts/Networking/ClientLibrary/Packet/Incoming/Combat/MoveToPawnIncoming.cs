using UnityEngine;

[IncomingGamePacket(GameServerPacketType.MoveToPawn)]
public sealed class MoveToPawnIncoming : IncomingWirePacket<MoveToPawnDto>
{
    public override void Apply(MoveToPawnDto packet) =>
        CombatIncomingPacketServices.ApplyMoveToPawn(packet);
}
