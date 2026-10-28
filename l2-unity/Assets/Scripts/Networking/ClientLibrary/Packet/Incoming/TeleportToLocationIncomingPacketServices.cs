using System;

/// <summary>
/// Teleport-to-location wire packet handler entry (#142 batch slice 143).
/// </summary>
public static class TeleportToLocationIncomingPacketServices
{
    public static World GameWorld => WorldIncomingPacketServices.GameWorld;

    public static void Queue(Action action) => WorldIncomingPacketServices.Queue(action);

    public static void ApplyTeleportToLocation(TeleportToLocationDto packet)
    {
        Queue(() => GameWorld.TeleportTo(packet.TarObjId, packet.TeleportPos));
    }
}
