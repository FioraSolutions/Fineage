using System;

/// <summary>
/// Message packet-apply access with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 46).
/// </summary>
public static class MessagePacketServices
{
    private static MessagePacketApply _messageApply;

    public static void Bind(MessagePacketApply messageApply = null)
    {
        if (messageApply != null)
            _messageApply = messageApply;
    }

    public static MessagePacketApply MessageApply =>
        _messageApply != null ? _messageApply : IncomingPacketServices.MessageApply;

    public static void ApplyMessage(Action<MessagePacketApply> apply)
    {
        MessagePacketApply messageApply = MessageApply;
        if (messageApply != null && apply != null)
            apply(messageApply);
    }
}
