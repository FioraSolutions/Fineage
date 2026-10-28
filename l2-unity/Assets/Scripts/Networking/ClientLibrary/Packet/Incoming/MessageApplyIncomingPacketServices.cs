using System;

/// <summary>
/// Message packet apply hub (#142 batch slice 159).
/// </summary>
public static class MessageApplyIncomingPacketServices
{
    public static MessagePacketApply MessageApply =>
        IncomingPacketLocatorApplyIncomingPacketServices.MessageApply;

    public static void ApplyMessage(Action<MessagePacketApply> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.ApplyMessage(apply);
}
