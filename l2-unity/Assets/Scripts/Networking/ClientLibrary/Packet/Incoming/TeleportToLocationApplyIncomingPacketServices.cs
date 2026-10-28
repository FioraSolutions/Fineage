using System;

/// <summary>
/// Apply facade (#142 batch slice 152). Re-homes from <see cref="TeleportToLocationIncomingPacketServices"/>.
/// 
/// Teleport-to-location wire packet handler entry (#142 batch slice 143).
/// </summary>
public static class TeleportToLocationApplyIncomingPacketServices
{
    public static World GameWorld => WorldApplyIncomingPacketServices.GameWorld;

    public static void Queue(Action action) => WorldApplyIncomingPacketServices.Queue(action);

    public static void ApplyTeleportToLocation(TeleportToLocationDto packet)
    {
        Queue(() => GameWorld.TeleportTo(packet.TarObjId, packet.TeleportPos));
    }
}
