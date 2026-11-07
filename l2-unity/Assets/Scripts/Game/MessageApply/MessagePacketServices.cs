using System;

/// <summary>
/// Message packet-apply and chat window access with [Inject] wiring and LocatorApply fallback (#142).
/// </summary>
public static class MessagePacketServices
{
    private static MessagePacketApply _messageApply;
    private static ChatWindow _chat;

    public static void Bind(MessagePacketApply messageApply = null, ChatWindow chat = null)
    {
        if (messageApply != null)
            _messageApply = messageApply;
        if (chat != null)
            _chat = chat;
    }

    public static MessagePacketApply MessageApply =>
        _messageApply != null ? _messageApply : LocatorApplyIncomingPacketServices.MessageApply;

    public static ChatWindow Chat =>
        _chat != null ? _chat : ChatWindow.Instance;

    public static void ApplyMessage(Action<MessagePacketApply> apply)
    {
        MessagePacketApply messageApply = MessageApply;
        if (messageApply != null && apply != null)
            apply(messageApply);
    }
}
