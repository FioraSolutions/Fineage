using System;

/// <summary>
/// Magic skill canceled wire packet handler entry (#142 batch slice 144).
/// </summary>
public static class MagicSkillCanceledIncomingPacketServices
{
    public static void ApplyMagicSkillCanceled(MagicSkillCanceledDto packet) =>
        WorldApplyPacketServices.ApplyWorld(apply => apply.MagicSkillCanceled(packet));
}
