using System;

/// <summary>
/// Magic skill use wire packet handler entry (#142 batch slice 144).
/// Apply facade (#142 batch slice 153). Re-homes from <see cref="MagicSkillUseIncomingPacketServices"/>.
/// </summary>
public static class MagicSkillUseApplyIncomingPacketServices
{
    public static void ApplyMagicSkillUse(MagicSkillUseDto packet)
    {
        // Temporary: skill cast/VFX hides CharInfo spawn cost in packetApply.txt.
        // Packet is still read, so the stream stays aligned. Restore the queue below to turn it back on.
        if (packet == null)
            return;

        CombatPacketApplyIncomingPacketServices.QueueWorld(apply => apply.MagicSkillUse(packet));
    }
}
