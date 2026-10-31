using System;

public sealed class PacketApplyQueue
{
    [ThreadStatic]
    private static int _applyDepth;

    private readonly EventProcessor _events;

    public PacketApplyQueue(EventProcessor events)
    {
        _events = events;
    }

    public void Queue(Action action)
    {
        if (_applyDepth > 0)
        {
            action();
            return;
        }

        QueueNextFrame(action);
    }

    /// <summary>
    /// Always enqueue for the next EventProcessor batch. Does not run inline
    /// during QueueApply (needed to defer CharInfo spawn without looping).
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
