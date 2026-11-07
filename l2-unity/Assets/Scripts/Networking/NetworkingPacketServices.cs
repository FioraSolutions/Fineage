using System;
using VContainer;

/// <summary>
/// Networking-layer service access with optional Bind wiring and container/live fallback (#142).
/// </summary>
public static class NetworkingPacketServices
{
    static World _world;
    static WorldPacketApply _worldApply;
    static IWorldDomainSink _domain;
    static GameManager _manager;
    static LoginClient _login;
    static LoginWindow _loginWindow;
    static PlayerController _player;
    static EventProcessor _events;
    static NetSession _session;

    public static void Bind(
        World world = null,
        WorldPacketApply worldApply = null,
        IWorldDomainSink domain = null,
        GameManager manager = null,
        LoginClient login = null,
        LoginWindow loginWindow = null,
        PlayerController player = null,
        EventProcessor events = null,
        NetSession session = null)
    {
        if (world != null)
            _world = world;
        if (worldApply != null)
            _worldApply = worldApply;
        if (domain != null)
            _domain = domain;
        if (manager != null)
            _manager = manager;
        if (login != null)
            _login = login;
        if (loginWindow != null)
            _loginWindow = loginWindow;
        if (player != null)
            _player = player;
        if (events != null)
            _events = events;
        if (session != null)
            _session = session;
    }

    public static NetSession Session
    {
        get
        {
            if (_session != null)
                return _session;
#pragma warning disable CS0618
            return NetSession.Instance;
#pragma warning restore CS0618
        }
    }

    public static EventProcessor Events =>
        _events != null ? _events : ResolveFromApp<EventProcessor>();

    public static PlayerController Player =>
        _player != null ? _player : ResolveFromGameContainer<PlayerController>();

    public static World GameWorld => ResolveWorld();

    public static WorldPacketApply WorldApply => ResolveWorldApply();

    public static IWorldDomainSink Domain => ResolveDomain();

    public static GameManager Manager =>
        _manager != null ? _manager : ResolveFromApp<GameManager>();

    public static LoginClient Login =>
        _login != null ? _login : ResolveFromApp<LoginClient>() ?? LoginClient.Instance;

    public static LoginWindow LoginWindow =>
        _loginWindow != null
            ? _loginWindow
            : ResolveFromApp<LoginWindow>() ?? LegacyLoginWindowInstance();

    public static void ApplyWorld(Action<WorldPacketApply> apply)
    {
        WorldPacketApply worldApply = WorldApply;
        if (worldApply != null && apply != null)
            apply(worldApply);
    }

    public static void Queue(Action action)
    {
        if (action == null)
            return;

        PacketApplyQueue applyQueue = ResolveFromApp<PacketApplyQueue>();
        if (applyQueue != null)
        {
            applyQueue.Queue(action);
            return;
        }

        EventProcessor processor = Events;
        if (processor != null)
            processor.QueueEvent(action);
    }

    public static void QueueNextFrame(Action action)
    {
        if (action == null)
            return;

        PacketApplyQueue applyQueue = ResolveFromApp<PacketApplyQueue>();
        if (applyQueue != null)
        {
            applyQueue.QueueNextFrame(action);
            return;
        }

        Queue(action);
    }

    static World ResolveWorld()
    {
        if (_world != null)
            return _world;
        return ResolveFromGameContainer<World>();
    }

    static WorldPacketApply ResolveWorldApply()
    {
        if (_worldApply != null)
            return _worldApply;
        return ResolveFromGameContainer<WorldPacketApply>();
    }

    static IWorldDomainSink ResolveDomain()
    {
        if (_domain != null)
            return _domain;
        return ResolveFromGameContainer<IWorldDomainSink>();
    }

    static T ResolveFromGameContainer<T>() where T : class
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

    static T ResolveFromApp<T>() where T : class
    {
        if (!App.HasContainer)
            return null;

        try
        {
            return App.Resolve<T>();
        }
        catch
        {
            return null;
        }
    }

#pragma warning disable CS0618 // Locator fallback until all call sites use LoginPacketServices
    static LoginWindow LegacyLoginWindowInstance() => LoginWindow.Instance;
#pragma warning restore CS0618
}
