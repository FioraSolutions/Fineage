using System;
using UnityEngine;

/// <summary>
/// Main-thread apply queue backed by app-lifetime <see cref="EventProcessor"/>
/// (hosted on AppLifetimeScope). Deferred work must never run inline — that recurses
/// when <see cref="SpawnFrameBudget"/> is exhausted.
/// </summary>
public sealed class PacketApplyQueue
{
    [ThreadStatic]
    private static int _applyDepth;

    public PacketApplyQueue(EventProcessor events)
    {
        if (events != null)
            NetworkingPacketServices.Bind(events: events);
    }

    public void Queue(Action action)
    {
        if (action == null)
            return;
        if (_applyDepth > 0)
        {
            action();
            return;
        }

        QueueNextFrame(action);
    }

    /// <summary>
    /// Enqueue for the next EventProcessor batch. Never runs synchronously.
    /// </summary>
    public void QueueNextFrame(Action action)
    {
        if (action == null)
            return;

        EventProcessor processor = NetworkingPacketServices.Events;
        if (processor == null)
        {
            Debug.LogError(
                "[PacketApplyQueue] QueueNextFrame dropped work: EventProcessor missing. "
                + "Expected on AppLifetimeScope (DiBootstrap.EnsureAppScope).");
            return;
        }

        processor.QueueEvent(action);
    }

    public void QueueApply(Action action)
    {
        if (action == null)
            return;
        Queue(() =>
        {
            _applyDepth++;
            try
            {
                action();
            }
            finally
            {
                _applyDepth--;
            }
        });
    }
}
