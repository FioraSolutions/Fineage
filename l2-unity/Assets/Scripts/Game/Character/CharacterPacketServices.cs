/// <summary>
/// Lobby character layer access with [Inject] wiring and Game fallback (#142).
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
