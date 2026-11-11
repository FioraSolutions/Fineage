using System;
using VContainer;
using System.Collections.Generic;

public static partial class AnimationPacketServices
{
    static PlayerAnimationController LegacyPlayerAnimationInstance() => PlayerAnimationController.Instance;

    static StriderRideService LegacyStriderRideInstance() => StriderRideService.Instance;

    public static void QueueOnMain(Action action)
    {
        if (action == null)
            return;

        PacketApplyQueue queue = _applyQueue;
        if (queue != null)
        {
            queue.Queue(action);
            return;
        }

        NetworkingPacketServices.Queue(action);
    }
}
