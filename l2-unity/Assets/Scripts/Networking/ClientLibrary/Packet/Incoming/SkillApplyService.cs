using System;

/// <summary>
/// Skill list and magic skill use applies, constructed by the container instead of reached
/// through static facades (#142 slice 3).
/// </summary>
public sealed class SkillApplyService
{
    public void ApplySkillList(SkillListDto packet) =>
        QueueApplyIncomingPacketServices.Queue(
            () => LocatorApplyIncomingPacketServices.SkillList.UpdateSkillList(packet.Skills));

    public void ApplyMagicSkillUse(MagicSkillUseDto packet)
    {
        // Temporary: skill cast/VFX hides CharInfo spawn cost in packetApply.txt.
        // Packet is still read, so the stream stays aligned.
        if (packet == null)
            return;

        QueueApplyIncomingPacketServices.QueueWorld(apply => apply.MagicSkillUse(packet));
    }
}
