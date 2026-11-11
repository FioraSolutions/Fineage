using System;
using VContainer;

public static partial class NetworkingPacketServices
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
#pragma warning restore CS0618
}
