using VContainer;

/// <summary>
/// Game-layer access to <see cref="UnityMainThreadDispatcher"/> (#131 C2).
/// </summary>
public static class DispatcherPacketServices
{
    static UnityMainThreadDispatcher _dispatcher;

    public static void Bind(UnityMainThreadDispatcher dispatcher = null)
    {
        if (dispatcher != null)
            _dispatcher = dispatcher;
    }

    public static bool Exists =>
        _dispatcher != null || UnityMainThreadDispatcher.Exists();

    public static UnityMainThreadDispatcher Dispatcher
    {
        get
        {
            if (_dispatcher != null)
                return _dispatcher;
            UnityMainThreadDispatcher resolved = ResolveFromGame<UnityMainThreadDispatcher>();
            if (resolved != null)
                return resolved;
#pragma warning disable CS0618 // Locator fallback until scene bind
            return UnityMainThreadDispatcher.Instance();
#pragma warning restore CS0618
        }
    }

    static T ResolveFromGame<T>() where T : class
    {
        if (App.GameContainer == null)
            return null;
        try { return App.GameContainer.Resolve<T>(); }
        catch { return null; }
    }
}
