using System;

/// <summary>
/// Auto-attack stop wire packet handler entry (#142 batch slice 144).
/// Apply facade (#142 batch slice 153). Re-homes from <see cref="AutoAttackStopIncomingPacketServices"/>.
/// </summary>
public static class AutoAttackStopApplyIncomingPacketServices
{
    public static void ApplyAutoAttackStop(AutoAttackStopDto packet) =>
        CombatPacketApplyIncomingPacketServices.ApplyWorld(apply => apply.AutoAttackStop(packet));
}
