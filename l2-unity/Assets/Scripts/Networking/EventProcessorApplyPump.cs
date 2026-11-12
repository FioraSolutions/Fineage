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
        _events = events ?? throw new ArgumentNullException(nameof(events));
        NetworkingPacketServices.Bind(events: events);
    }

    public void QueueOnMainThread(Action action)
    {
        if (action == null)
            return;

        EventProcessor processor = _events;
        if (processor == null)
            processor = NetworkingPacketServices.Events;
        if (processor == null)
        {
            Debug.LogError(
                "[EventProcessorApplyPump] EventProcessor missing — drop deferred apply (no EnsureInstance fallback)");
            return;
        }

        processor.QueueEvent(action);
    }
}
