using System;

/// <summary>
/// Magic skill canceled wire packet handler entry (#142 batch slice 144).
/// Apply facade (#142 batch slice 153). Re-homes from <see cref="MagicSkillCanceledIncomingPacketServices"/>.
/// </summary>
public static class MagicSkillCanceledApplyIncomingPacketServices
{
    public static void ApplyMagicSkillCanceled(MagicSkillCanceledDto packet) =>
        CombatSessionApplyIncomingPacketServices.ApplyWorld(apply => apply.MagicSkillCanceled(packet));
}
