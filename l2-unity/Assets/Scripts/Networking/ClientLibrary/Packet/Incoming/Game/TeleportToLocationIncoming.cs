using UnityEngine;

[IncomingGamePacket(GameServerPacketType.TeleportToLocation)]
public sealed class TeleportToLocationIncoming : IncomingWirePacket<TeleportToLocationDto>
{
    public override void Apply(TeleportToLocationDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            // UnityChainLog.OnTeleport(packet.TarObjId, packet.X, packet.Y, packet.Z);
            IncomingPacketServices.GameWorld.TeleportTo(packet.TarObjId, packet.TeleportPos);
        });
    }
}
