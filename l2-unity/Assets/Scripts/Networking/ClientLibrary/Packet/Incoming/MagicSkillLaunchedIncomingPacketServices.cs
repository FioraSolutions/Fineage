using System;

/// <summary>
/// Magic skill launched wire packet handler entry (#142 batch slice 144).
/// </summary>
public static class MagicSkillLaunchedIncomingPacketServices
{
    public static void ApplyMagicSkillLaunched(MagicSkillLaunchedDto packet)
    {
        if (packet == null)
            return;

        WorldApplyPacketServices.QueueWorld(apply => apply.MagicSkillLaunched(packet));
    }
}
