using System;
using System.Collections.Generic;

public static partial class WorldApplyPacketServices
{
    public static void QueueWorld(Action<WorldPacketApply> apply)
    {
        if (apply == null)
            return;
        Queue(() => ApplyWorld(apply));
    }

    public static void Queue(Action action) => NetworkingPacketServices.Queue(action);

    public static void QueueWorldNextFrame(Action<WorldPacketApply> apply)
    {
        if (apply == null)
            return;
        NetworkingPacketServices.QueueNextFrame(() => ApplyWorld(apply));
    }
}
