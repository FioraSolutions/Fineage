using System;

/// <summary>
/// Creature say wire packet handler entry (#142 batch slice 143).
/// </summary>
public static class CreatureSayIncomingPacketServices
{
    public static void ApplyCreatureSay(CreatureSayDto packet)
    {
        if (packet.Message == null)
            return;

        ApplyCreatureSay(packet.Message);
    }

    public static void ApplyCreatureSay(CreatureMessage message) =>
        MessagePacketServices.ApplyMessage(apply => apply.CreatureSay(message));
}
