using System;

/// <summary>
/// Apply facade (#142 batch slice 152). Re-homes from <see cref="SystemMessageIncomingPacketServices"/>.
/// 
/// System message wire packet handler entry (#142 batch slice 143).
/// </summary>
public static class SystemChatApplyIncomingPacketServices
{
    public static void ApplySystemMessage(SystemMessageDto packet)
    {
        try
        {
            ChatApplyIncomingPacketServices.ApplyMessage(apply => apply.SystemMessage(packet));
        }
        catch (Exception)
        {
        }
    }
}
