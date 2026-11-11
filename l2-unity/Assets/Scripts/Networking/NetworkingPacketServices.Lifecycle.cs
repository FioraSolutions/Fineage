using System;
using VContainer;

public static partial class NetworkingPacketServices
{
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
}
