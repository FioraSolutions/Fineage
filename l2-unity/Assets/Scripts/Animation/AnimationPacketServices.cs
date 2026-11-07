using System;

/// <summary>
/// Animation-layer access with [Inject] wiring and peer PacketServices fallback (#142).
/// </summary>
public static class AnimationPacketServices
{
    private static IAnimationManager _animations;
    private static PacketApplyQueue _applyQueue;

    public static void Bind(
        IAnimationManager animations = null,
        PacketApplyQueue applyQueue = null)
    {
        if (animations != null)
            _animations = animations;
        if (applyQueue != null)
            _applyQueue = applyQueue;
    }

    public static IAnimationManager Animations =>
        _animations != null ? _animations : GamePacketServices.Animations;

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
