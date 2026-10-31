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

        // Never run inline: UpdateNpc re-queues itself when SpawnFrameBudget is exhausted,
        // so an inline fallback recursed until the main-thread stack overflowed (Standalone SIGSEGV).
        EventProcessor processor = _events != null ? _events : NetworkingPacketServices.Events;
        if (processor == null)
            processor = EventProcessor.EnsureInstance();
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
