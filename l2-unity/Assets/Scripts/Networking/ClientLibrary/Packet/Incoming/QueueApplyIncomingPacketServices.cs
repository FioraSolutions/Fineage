using System;

/// <summary>
/// Apply queue relay facade for incoming handler hubs (#142 batch slice 172).
/// </summary>
public static class QueueApplyIncomingPacketServices
{
    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.ApplyWorld(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.QueueWorld(apply);

    public static void ApplyDomain(Action<IWorldDomainSink> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.ApplyDomain(apply);

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.QueueDomain(apply);

    public static void ApplyMessage(Action<MessagePacketApply> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.ApplyMessage(apply);

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);

    public static void Bind(PacketApplyQueue applyQueue = null, EventProcessor events = null) =>
        IncomingPacketQueueApplyIncomingPacketServices.Bind(applyQueue: applyQueue, events: events);
}
