using VContainer;

/// <summary>
/// Game-layer access to shared services with [Inject] wiring and Networking/container fallback (#142).
/// </summary>
public static class GamePacketServices
{
    private static World _gameWorld;
    private static EventBus _bus;
    private static GameManager _manager;
    private static IAnimationManager _animations;

    public static void Bind(
        World gameWorld = null,
        EventBus bus = null,
        GameManager manager = null,
        IAnimationManager animations = null)
    {
        if (gameWorld != null)
            _gameWorld = gameWorld;
        if (bus != null)
            _bus = bus;
        if (manager != null)
            _manager = manager;
        if (animations != null)
            _animations = animations;
    }

    public static World GameWorld =>
        _gameWorld != null ? _gameWorld : NetworkingPacketServices.GameWorld;

    public static EventBus Bus
    {
        get
        {
            if (_bus != null)
                return _bus;
            EventBus resolved = ResolveFromGame<EventBus>();
            if (resolved != null)
                return resolved;
#pragma warning disable CS0618 // Locator fallback until drain completes
            return EventBus.Instance;
#pragma warning restore CS0618
        }
    }

    public static GameManager Manager =>
        _manager != null ? _manager : NetworkingPacketServices.Manager;

    public static IAnimationManager Animations =>
        _animations != null ? _animations : ResolveAnimationsFallback();

    static IAnimationManager ResolveAnimationsFallback() =>
        ResolveFromGame<IAnimationManager>() ?? AnimationPacketServices.LegacyAnimationManagerInstance();

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
