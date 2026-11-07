using System;

/// <summary>
/// Animation-layer access with [Inject] wiring and peer PacketServices fallback (#142).
/// </summary>
public static class AnimationPacketServices
{
    private static IAnimationManager _animations;
    private static IMoveAllCharacters _moves;
    private static PacketApplyQueue _applyQueue;

    public static void Bind(
        IAnimationManager animations = null,
        IMoveAllCharacters moves = null,
        PacketApplyQueue applyQueue = null)
    {
        if (animations != null)
            _animations = animations;
        if (moves != null)
            _moves = moves;
        if (applyQueue != null)
            _applyQueue = applyQueue;
    }

    public static IAnimationManager Animations =>
        _animations != null ? _animations : GamePacketServices.Animations;

    public static IMoveAllCharacters Moves =>
        _moves != null ? _moves : MoveAllCharactersPacketServices.Moves;

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
