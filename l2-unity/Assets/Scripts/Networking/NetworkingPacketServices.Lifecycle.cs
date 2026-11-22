using System;
using System.Collections.Generic;
using UnityEngine;
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

    /// <summary>
    /// Cache the bound world when the caller only has <see cref="IWorldSpawnContext"/>.
    /// A null context is a no-op. Anything else than <see cref="World"/> logs and is not stored.
    /// </summary>
    public static void BindSpawnContext(IWorldSpawnContext world)
    {
        if (world == null)
            return;
        if (world is World concrete)
        {
            _world = concrete;
            return;
        }

        Debug.LogError("[NetworkingPacketServices] IWorldSpawnContext is not a World — GameLifetimeScope.RegisterWorld registered a different type, so the world host was not stored.");
    }

    /// <summary>
    /// Clear App-owned static hosts when composition root dies (GPT A08) — reference-equals only.
    /// </summary>
    public static void Unbind(
        GameManager manager = null,
        LoginClient login = null,
        EventProcessor events = null,
        NetSession session = null)
    {
        if (manager != null && ReferenceEquals(_manager, manager))
            _manager = null;
        if (login != null && ReferenceEquals(_login, login))
            _login = null;
        if (events != null && ReferenceEquals(_events, events))
            _events = null;
        if (session != null && ReferenceEquals(_session, session))
            _session = null;
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
