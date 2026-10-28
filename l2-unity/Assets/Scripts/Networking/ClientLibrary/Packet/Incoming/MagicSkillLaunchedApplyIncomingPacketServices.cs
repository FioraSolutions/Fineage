using System;

/// <summary>
/// Magic skill launched wire packet handler entry (#142 batch slice 144).
/// Apply facade (#142 batch slice 153). Re-homes from <see cref="MagicSkillLaunchedIncomingPacketServices"/>.
/// </summary>
public static class MagicSkillLaunchedApplyIncomingPacketServices
{
    public static void ApplyMagicSkillLaunched(MagicSkillLaunchedDto packet)
    {
        if (packet == null)
            return;

        WorldApplyPacketServices.QueueWorld(apply => apply.MagicSkillLaunched(packet));
    }
}
