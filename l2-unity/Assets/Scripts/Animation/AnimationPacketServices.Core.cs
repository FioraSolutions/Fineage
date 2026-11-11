using System;
using VContainer;

public static partial class AnimationPacketServices
{
    public static void Bind(
        IAnimationManager animations = null,
        IMoveAllCharacters moves = null,
        PacketApplyQueue applyQueue = null,
        PlayerAnimationController playerAnimation = null,
        StriderRideService striderRide = null)
    {
        if (animations != null)
            _animations = animations;
        if (moves != null)
            _moves = moves;
        if (applyQueue != null)
            _applyQueue = applyQueue;
        if (playerAnimation != null)
            _playerAnimation = playerAnimation;
        if (striderRide != null)
            _striderRide = striderRide;
    }

    internal static IAnimationManager LegacyAnimationManagerInstance() => AnimationManager.Instance;
}
