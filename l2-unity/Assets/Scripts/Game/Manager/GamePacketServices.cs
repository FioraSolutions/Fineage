using VContainer;

public static partial class GamePacketServices
{
    private static World _gameWorld;
    private static EventBus _bus;
    private static GameManager _manager;
    private static IAnimationManager _animations;


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
