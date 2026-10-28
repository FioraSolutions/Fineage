using System;

/// <summary>
/// Magic skill use wire packet handler entry (#142 batch slice 144).
/// </summary>
public static class MagicSkillUseIncomingPacketServices
{
    public static void ApplyMagicSkillUse(MagicSkillUseDto packet)
    {
        // Temporary: skill cast/VFX hides CharInfo spawn cost in packetApply.txt.
        // Packet is still read, so the stream stays aligned. Restore the queue below to turn it back on.
        if (packet == null)
            return;

        WorldApplyPacketServices.QueueWorld(apply => apply.MagicSkillUse(packet));
    }
}
