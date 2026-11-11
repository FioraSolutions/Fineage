/// <summary>
/// Status attribute apply, constructed by the container instead of reached through the
/// static <c>*ApplyIncomingPacketServices</c> relay (#142 slice 3 pilot).
/// </summary>
using System.Collections.Generic;
using System;

public sealed class StatusUpdateApplyService
{
    public World GameWorld => WorldApplyIncomingPacketServices.GameWorld;

    public void Apply(int objectId, System.Collections.Generic.List<StatusUpdate.Attribute> attributes)
    {
        World gameWorld = GameWorld;
        if (gameWorld != null)
            gameWorld.StatusUpdate(objectId, attributes);
    }
}
