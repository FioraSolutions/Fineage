using System;
using VContainer;

public static partial class AnimationPacketServices
{
    private static IAnimationManager _animations;
    private static IMoveAllCharacters _moves;
    private static PacketApplyQueue _applyQueue;
    private static PlayerAnimationController _playerAnimation;
    private static StriderRideService _striderRide;


    public static IAnimationManager Animations =>
        _animations != null ? _animations : GamePacketServices.Animations;

#pragma warning disable CS0618 // Locator fallback until all call sites are [Inject]
#pragma warning restore CS0618

    public static IMoveAllCharacters Moves =>
        _moves != null ? _moves : MoveAllCharactersPacketServices.Moves;

    public static PlayerAnimationController PlayerAnimation =>
        _playerAnimation != null
            ? _playerAnimation
            : ResolveFromGame<PlayerAnimationController>() ?? LegacyPlayerAnimationInstance();

    public static StriderRideService StriderRide =>
        _striderRide != null
            ? _striderRide
            : ResolveFromGame<StriderRideService>() ?? LegacyStriderRideInstance();

    static T ResolveFromGame<T>() where T : class
    {
        if (App.GameContainer == null)
            return null;

        try
        {
            return App.GameContainer.Resolve<T>();
        }
        catch
        {
            return null;
        }
    }

#pragma warning disable CS0618 // Locator fallback until all call sites are [Inject]

#pragma warning restore CS0618

}
