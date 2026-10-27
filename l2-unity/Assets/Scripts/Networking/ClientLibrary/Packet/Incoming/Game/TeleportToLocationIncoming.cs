using UnityEngine;

[IncomingGamePacket(GameServerPacketType.TeleportToLocation)]
public sealed class TeleportToLocationIncoming : IncomingWirePacket<TeleportToLocationDto>
{
    public override void Apply(TeleportToLocationDto packet)
    {
        WorldIncomingPacketServices.Queue(() =>
        {
            // UnityChainLog.OnTeleport(packet.TarObjId, packet.X, packet.Y, packet.Z);
            WorldIncomingPacketServices.GameWorld.TeleportTo(packet.TarObjId, packet.TeleportPos);
        });
    }
}
