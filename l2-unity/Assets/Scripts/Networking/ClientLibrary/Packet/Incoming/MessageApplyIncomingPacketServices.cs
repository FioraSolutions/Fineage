using System;

/// <summary>
/// Message packet apply hub (#142 batch slice 159; queue slice 166).
/// </summary>
public static class MessageApplyIncomingPacketServices
{
    public static MessagePacketApply MessageApply =>
        IncomingPacketLocatorApplyIncomingPacketServices.MessageApply;

    public static void ApplyMessage(Action<MessagePacketApply> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.ApplyMessage(apply);

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}
