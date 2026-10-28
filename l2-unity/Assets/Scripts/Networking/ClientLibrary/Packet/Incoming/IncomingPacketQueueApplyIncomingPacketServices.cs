using System;

/// <summary>
/// Queue and world/domain/message apply dispatch for incoming handlers (#142 batch slice 158).
/// Re-homes apply/queue methods from <see cref="IncomingPacketServices"/>.
/// </summary>
public static class IncomingPacketQueueApplyIncomingPacketServices
{
    private static PacketApplyQueue _applyQueue;
    private static EventProcessor _events;

    public static void Bind(PacketApplyQueue applyQueue = null, EventProcessor events = null)
    {
        if (applyQueue != null)
            _applyQueue = applyQueue;
        if (events != null)
            _events = events;
    }

    public static void ApplyWorld(Action<WorldPacketApply> apply)
    {
        WorldPacketApply worldApply = IncomingPacketLocatorApplyIncomingPacketServices.WorldApply;
        if (worldApply != null && apply != null)
            apply(worldApply);
    }

    public static void QueueWorld(Action<WorldPacketApply> apply)
    {
        if (apply == null)
            return;
        Queue(() => ApplyWorld(apply));
    }

    public static void ApplyDomain(Action<IWorldDomainSink> apply)
    {
        IWorldDomainSink domain = IncomingPacketLocatorApplyIncomingPacketServices.Domain;
        if (domain != null && apply != null)
            apply(domain);
    }

    public static void QueueDomain(Action<IWorldDomainSink> apply)
    {
        if (apply == null)
            return;
        Queue(() => ApplyDomain(apply));
    }

    public static void ApplyMessage(Action<MessagePacketApply> apply)
    {
        MessagePacketApply messageApply = IncomingPacketLocatorApplyIncomingPacketServices.MessageApply;
        if (messageApply != null && apply != null)
            apply(messageApply);
    }

    public static void Queue(Action action)
    {
        if (_applyQueue != null)
        {
            _applyQueue.Queue(action);
            return;
        }

        EventProcessor processor = _events != null ? _events : EventProcessor.Instance;
        if (processor != null)
            processor.QueueEvent(action);
        else
            IncomingPacketActions.Queue(action);
    }
}
