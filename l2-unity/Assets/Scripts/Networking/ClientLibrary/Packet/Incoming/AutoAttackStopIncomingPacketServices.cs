using System;

/// <summary>
/// Auto-attack stop wire packet handler entry (#142 batch slice 144).
/// </summary>
public static class AutoAttackStopIncomingPacketServices
{
    public static void ApplyAutoAttackStop(AutoAttackStopDto packet) =>
        WorldApplyPacketServices.ApplyWorld(apply => apply.AutoAttackStop(packet));
}
