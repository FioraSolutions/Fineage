using System;

/// <summary>
/// Chat / system message apply hub (#142 batch slice 168; queue relay slice 253).
/// </summary>
public static class ChatApplyIncomingPacketServices
{
    public static void ApplyMessage(Action<MessagePacketApply> apply) =>
        QueueApplyIncomingPacketServices.ApplyMessage(apply);

    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);
}
