/// <summary>
/// Apply facade (#142 batch slice 152). Re-homes from <see cref="StatusUpdateIncomingPacketServices"/>.
/// 
/// Status update wire packet handler entry (#142 batch slice 143).
/// </summary>
using System.Collections.Generic;
using System;

public static class StatusUpdateApplyIncomingPacketServices
{
    public static World GameWorld => WorldApplyIncomingPacketServices.GameWorld;

    public static void ApplyStatusUpdate(int objectId, System.Collections.Generic.List<StatusUpdate.Attribute> attributes)
    {
        if (GameWorld != null)
            GameWorld.StatusUpdate(objectId, attributes);
    }
}
