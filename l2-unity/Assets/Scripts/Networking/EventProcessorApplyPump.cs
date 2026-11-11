using System;

/// <summary>
/// ACS bridge: <see cref="EventProcessor"/> + <see cref="NetworkingPacketServices"/> as
/// <see cref="IMainThreadApplyPump"/> for <see cref="PacketApplyQueue"/> (#131).
/// </summary>
public sealed class EventProcessorApplyPump : IMainThreadApplyPump
{
    public EventProcessorApplyPump(EventProcessor events)
    {
        if (events != null)
            NetworkingPacketServices.Bind(events: events);
    }

    public void QueueOnMainThread(Action action)
    {
        if (action == null)
            return;

        EventProcessor processor = NetworkingPacketServices.Events;
        if (processor == null)
            processor = EventProcessor.EnsureInstance();
        processor.QueueEvent(action);
    }
}
