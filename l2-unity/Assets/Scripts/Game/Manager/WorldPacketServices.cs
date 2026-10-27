/// <summary>
/// World runtime access to shared services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 40).
/// </summary>
public static class WorldPacketServices
{
    private static IAnimationManager _animations;
    private static GameClient _game;
    private static GameManager _manager;
    private static WorldPacketApply _worldApply;

    public static void Bind(
        IAnimationManager animations = null,
        GameClient game = null,
        GameManager manager = null,
        WorldPacketApply worldApply = null)
    {
        if (animations != null)
            _animations = animations;
        if (game != null)
            _game = game;
        if (manager != null)
            _manager = manager;
        if (worldApply != null)
            _worldApply = worldApply;
    }

    public static IAnimationManager Animations =>
        _animations != null ? _animations : IncomingPacketActions.Animations;

    public static GameClient Game =>
        _game != null ? _game : IncomingPacketActions.Game;

    public static GameManager Manager =>
        _manager != null ? _manager : IncomingPacketActions.Manager;

    public static WorldPacketApply WorldApply =>
        _worldApply != null ? _worldApply : IncomingPacketActions.WorldApply;
}
