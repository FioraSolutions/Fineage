using System;
using VContainer;

/// <summary>
/// Networking-layer service access with optional Bind wiring and IncomingPacketActions fallback (#142).
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

    public static void Bind(
        World world = null,
        WorldPacketApply worldApply = null,
        IWorldDomainSink domain = null,
        GameManager manager = null,
        LoginClient login = null,
        LoginWindow loginWindow = null,
        PlayerController player = null,
        EventProcessor events = null)
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
    }

    public static EventProcessor Events =>
        _events != null ? _events : ResolveFromGameContainer<EventProcessor>() ?? EventProcessor.Instance;

    public static PlayerController Player =>
        _player != null ? _player : ResolveFromGameContainer<PlayerController>() ?? IncomingPacketActions.Player;

    public static World GameWorld => ResolveWorld();

    public static WorldPacketApply WorldApply => ResolveWorldApply();

    public static IWorldDomainSink Domain => ResolveDomain();

    public static GameManager Manager =>
        _manager != null ? _manager : ResolveFromApp<GameManager>() ?? IncomingPacketActions.Manager;

    public static LoginClient Login =>
        _login != null ? _login : ResolveFromApp<LoginClient>() ?? IncomingPacketActions.Login;

    public static LoginWindow LoginWindow =>
        _loginWindow != null ? _loginWindow : ResolveFromApp<LoginWindow>() ?? IncomingPacketActions.LoginWindow;

    public static void ApplyWorld(Action<WorldPacketApply> apply)
    {
        WorldPacketApply worldApply = WorldApply;
        if (worldApply != null && apply != null)
            apply(worldApply);
    }

    static World ResolveWorld()
    {
        if (_world != null)
            return _world;
        World fromGame = ResolveFromGameContainer<World>();
        if (fromGame != null)
            return fromGame;
        return IncomingPacketActions.GameWorld;
    }

    static WorldPacketApply ResolveWorldApply()
    {
        if (_worldApply != null)
            return _worldApply;
        WorldPacketApply fromGame = ResolveFromGameContainer<WorldPacketApply>();
        if (fromGame != null)
            return fromGame;
        return IncomingPacketActions.WorldApply;
    }

    static IWorldDomainSink ResolveDomain()
    {
        if (_domain != null)
            return _domain;
        IWorldDomainSink fromGame = ResolveFromGameContainer<IWorldDomainSink>();
        if (fromGame != null)
            return fromGame;
        return IncomingPacketActions.Domain;
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
}
