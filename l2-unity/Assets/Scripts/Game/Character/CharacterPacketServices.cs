/// <summary>
/// Lobby character layer access with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 28).
/// </summary>
public static class CharacterPacketServices
{
    private static IAnimationManager _animations;

    public static void Bind(
        IAnimationManager animations = null)
    {
        if (animations != null)
            _animations = animations;
    }

    public static IAnimationManager Animations =>
        _animations != null ? _animations : GamePacketServices.Animations;
}
