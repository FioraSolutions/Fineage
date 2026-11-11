using System;

public static partial class WorldApplyPacketServices
{
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

    public static void ApplyWorld(Action<WorldPacketApply> apply)
    {
        WorldPacketApply worldApply = WorldApply;
        if (worldApply != null && apply != null)
            apply(worldApply);
    }
}
