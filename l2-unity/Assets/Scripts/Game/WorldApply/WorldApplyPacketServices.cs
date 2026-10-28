using System;

/// <summary>
/// World-apply layer access to game services with DI wiring and IncomingPacketActions fallback (#142 sub-slice 29).
/// </summary>
public static class WorldApplyPacketServices
{
    private static PlayerController _player;
    private static EventBus _bus;
    private static World _gameWorld;
    private static GameManager _manager;
    private static IWorldDomainSink _domain;
    private static WorldPacketApply _worldApply;

    public static void Bind(
        PlayerController player = null,
        EventBus bus = null,
        World gameWorld = null,
        GameManager manager = null,
        IWorldDomainSink domain = null,
        WorldPacketApply worldApply = null)
    {
        if (player != null)
            _player = player;
        if (bus != null)
            _bus = bus;
        if (gameWorld != null)
            _gameWorld = gameWorld;
        if (manager != null)
            _manager = manager;
        if (domain != null)
            _domain = domain;
        if (worldApply != null)
            _worldApply = worldApply;
    }

    public static PlayerController Player =>
        _player != null ? _player : IncomingPacketActions.Player;

    public static EventBus Bus =>
        _bus != null ? _bus : IncomingPacketActions.Bus;

    public static World GameWorld =>
        _gameWorld != null ? _gameWorld : IncomingPacketActions.GameWorld;

    public static GameManager Manager =>
        _manager != null ? _manager : IncomingPacketActions.Manager;

    public static IWorldDomainSink Domain =>
        _domain != null ? _domain : IncomingPacketActions.Domain;

    public static WorldPacketApply WorldApply =>
        _worldApply != null ? _worldApply : IncomingPacketActions.WorldApply;

    public static void ApplyWorld(Action<WorldPacketApply> apply)
    {
        WorldPacketApply worldApply = WorldApply;
        if (worldApply != null && apply != null)
            apply(worldApply);
    }

    public static void QueueWorld(Action<WorldPacketApply> apply)
    {
        if (apply == null)
            return;
        Queue(() => ApplyWorld(apply));
    }

    public static void Queue(Action action)
    {
        IncomingPacketActions.Queue(action);
    }

    public static void QueueWorldNextFrame(Action<WorldPacketApply> apply)
    {
        IncomingPacketActions.QueueWorldNextFrame(apply);
    }
}
