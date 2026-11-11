using System;

/// <summary>
/// Main-thread work pump for deferred packet/world apply (#131).
/// ACS <c>EventProcessor</c> implements this; <see cref="PacketApplyQueue"/> stays in Fineage.Networking.
/// </summary>
public interface IMainThreadApplyPump
{
    void QueueOnMainThread(Action action);
}
