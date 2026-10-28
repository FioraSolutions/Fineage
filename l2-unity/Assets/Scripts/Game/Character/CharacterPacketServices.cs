/// <summary>
/// Lobby character layer access with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 28).
/// </summary>
public static class CharacterPacketServices
{
    private static GameClient _game;
    private static IAnimationManager _animations;

    public static void Bind(
        GameClient game = null,
        IAnimationManager animations = null)
    {
        if (game != null)
            _game = game;
        if (animations != null)
            _animations = animations;
    }

    public static GameClient Game =>
        _game != null ? _game : IncomingPacketActions.Game;

    public static IAnimationManager Animations =>
        _animations != null ? _animations : IncomingPacketActions.Animations;
}
