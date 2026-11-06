using VContainer;

/// <summary>
/// Game-layer access to shared services with [Inject] wiring and Networking/container fallback (#142).
/// </summary>
public static class GamePacketServices
{
    private static World _gameWorld;
    private static EventBus _bus;
    private static EffectSkillsmanager _effectSkills;
    private static GameManager _manager;
    private static IAnimationManager _animations;

    public static void Bind(
        World gameWorld = null,
        EventBus bus = null,
        EffectSkillsmanager effectSkills = null,
        GameManager manager = null,
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
        if (animations != null)
            _animations = animations;
    }

    public static World GameWorld =>
        _gameWorld != null ? _gameWorld : NetworkingPacketServices.GameWorld;

    public static EventBus Bus =>
        _bus != null ? _bus : ResolveFromGame<EventBus>() ?? IncomingPacketActions.Bus;

    public static EffectSkillsmanager EffectSkills =>
        _effectSkills != null
            ? _effectSkills
            : ResolveFromGame<EffectSkillsmanager>() ?? IncomingPacketActions.EffectSkills;

    public static GameManager Manager =>
        _manager != null ? _manager : NetworkingPacketServices.Manager;

    public static IAnimationManager Animations =>
        _animations != null
            ? _animations
            : ResolveFromGame<IAnimationManager>() ?? IncomingPacketActions.Animations;

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
}
