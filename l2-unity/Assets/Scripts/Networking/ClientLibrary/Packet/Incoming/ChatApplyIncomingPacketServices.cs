using System;

/// <summary>
/// Chat / system message apply hub (#142 batch slice 168; MessageApply orchestration slice 207).
/// </summary>
public static class ChatApplyIncomingPacketServices
{
    public static void ApplyMessage(Action<MessagePacketApply> apply) =>
        MessageApplyIncomingPacketServices.ApplyMessage(apply);

    public static void Queue(Action action) => MessageApplyIncomingPacketServices.Queue(action);
}
