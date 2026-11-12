using System;
using UnityEngine;

/// <summary>
/// ACS bridge: injected <see cref="EventProcessor"/> as
/// <see cref="IMainThreadApplyPump"/> for <see cref="PacketApplyQueue"/> (#131 / GPT A10).
/// </summary>
public sealed class EventProcessorApplyPump : IMainThreadApplyPump
{
    readonly EventProcessor _events;

    public EventProcessorApplyPump(EventProcessor events)
    {
        // Events host is bound by AppLoginSessionApplyIncomingPacketServices.BindApp (A08/A10).
        _events = events ?? throw new ArgumentNullException(nameof(events));
    }

    public void QueueOnMainThread(Action action)
    {
        if (action == null)
            return;

        if (_events == null)
        {
            Debug.LogError(
                "[EventProcessorApplyPump] EventProcessor missing — drop deferred apply (no EnsureInstance fallback)");
            return;
        }

        _events.QueueEvent(action);
    }
}
