using System;

/// <summary>
/// Deferred apply queue used by App scope and packet apply services (#131).
/// </summary>
public interface IPacketApplyQueue
{
    void Queue(Action action);
    void QueueNextFrame(Action action);
    void QueueApply(Action action);
}
