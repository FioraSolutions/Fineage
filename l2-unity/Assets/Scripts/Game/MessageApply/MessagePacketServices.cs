using System;

/// <summary>
/// Message packet-apply access with [Inject] wiring and LocatorApply fallback (#142).
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
        _messageApply != null ? _messageApply : LocatorApplyIncomingPacketServices.MessageApply;

    public static void ApplyMessage(Action<MessagePacketApply> apply)
    {
        MessagePacketApply messageApply = MessageApply;
        if (messageApply != null && apply != null)
            apply(messageApply);
    }
}
