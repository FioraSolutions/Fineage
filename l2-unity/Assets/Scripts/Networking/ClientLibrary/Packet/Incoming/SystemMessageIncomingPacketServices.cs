using System;

/// <summary>
/// System message wire packet handler entry (#142 batch slice 143).
/// </summary>
public static class SystemMessageIncomingPacketServices
{
    public static void ApplySystemMessage(SystemMessageDto packet)
    {
        try
        {
            MessagePacketServices.ApplyMessage(apply => apply.SystemMessage(packet));
        }
        catch (Exception)
        {
        }
    }
}
