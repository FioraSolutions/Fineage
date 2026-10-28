using System;

/// <summary>
/// Auto-attack start wire packet handler entry (#142 batch slice 144).
/// Apply facade (#142 batch slice 153). Re-homes from <see cref="AutoAttackStartIncomingPacketServices"/>.
/// </summary>
public static class AutoAttackStartApplyIncomingPacketServices
{
    public static void ApplyAutoAttackStart(AutoAttackStartDto packet) =>
        CombatApplyIncomingPacketServices.ApplyWorld(apply => apply.AutoAttackStart(packet));
}
