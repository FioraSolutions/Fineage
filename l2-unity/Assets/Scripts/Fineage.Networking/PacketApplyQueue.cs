using System;

/// <summary>
/// Main-thread apply queue. Deferred work must never run inline — that recurses
/// when spawn frame budget is exhausted.
/// </summary>
public sealed class PacketApplyQueue : IPacketApplyQueue
{
    [ThreadStatic]
    private static int _applyDepth;

    private readonly IMainThreadApplyPump _pump;

    public PacketApplyQueue(IMainThreadApplyPump pump)
    {
        _pump = pump ?? throw new ArgumentNullException(nameof(pump));
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
    /// Enqueue for the next main-thread batch. Never runs synchronously.
    /// </summary>
    public void QueueNextFrame(Action action)
    {
        if (action == null)
            return;

        _pump.QueueOnMainThread(action);
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
