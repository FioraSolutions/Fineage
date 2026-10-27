/// <summary>
/// Game-layer access to shared services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 35).
/// </summary>
public static class GamePacketServices
{
    private static World _gameWorld;
    private static EventBus _bus;
    private static EffectSkillsmanager _effectSkills;
    private static GameManager _manager;
    private static GameClient _game;
    private static IAnimationManager _animations;

    public static void Bind(
        World gameWorld = null,
        EventBus bus = null,
        EffectSkillsmanager effectSkills = null,
        GameManager manager = null,
        GameClient game = null,
        IAnimationManager animations = null)
    {
        if (gameWorld != null)
            _gameWorld = gameWorld;
        if (bus != null)
            _bus = bus;
        if (effectSkills != null)
            _effectSkills = effectSkills;
        if (manager != null)
            _manager = manager;
        if (game != null)
            _game = game;
        if (animations != null)
            _animations = animations;
    }

    public static World GameWorld =>
        _gameWorld != null ? _gameWorld : IncomingPacketActions.GameWorld;

    public static EventBus Bus =>
        _bus != null ? _bus : IncomingPacketActions.Bus;

    public static EffectSkillsmanager EffectSkills =>
        _effectSkills != null ? _effectSkills : IncomingPacketActions.EffectSkills;

    public static GameManager Manager =>
        _manager != null ? _manager : IncomingPacketActions.Manager;

    public static GameClient Game =>
        _game != null ? _game : IncomingPacketActions.Game;

    public static IAnimationManager Animations =>
        _animations != null ? _animations : IncomingPacketActions.Animations;
}
