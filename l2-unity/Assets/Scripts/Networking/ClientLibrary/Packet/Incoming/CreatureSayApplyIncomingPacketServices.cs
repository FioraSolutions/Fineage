using System;

/// <summary>
/// Apply facade (#142 batch slice 152). Re-homes from <see cref="CreatureSayIncomingPacketServices"/>.
/// 
/// Creature say wire packet handler entry (#142 batch slice 143).
/// </summary>
public static class CreatureSayApplyIncomingPacketServices
{
    public static void ApplyCreatureSay(CreatureSayDto packet)
    {
        if (packet.Message == null)
            return;

        ApplyCreatureSay(packet.Message);
    }

    public static void ApplyCreatureSay(CreatureMessage message) =>
        ChatApplyIncomingPacketServices.ApplyMessage(apply => apply.CreatureSay(message));
}
