using System;

/// <summary>
/// Message packet apply hub (#142 batch slice 159; chat orchestration slice 177).
/// </summary>
public static class MessageApplyIncomingPacketServices
{
    public static MessagePacketApply MessageApply =>
        LocatorApplyIncomingPacketServices.MessageApply;

    public static void ApplyMessage(Action<MessagePacketApply> apply) =>
        QueueApplyIncomingPacketServices.ApplyMessage(apply);

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}
