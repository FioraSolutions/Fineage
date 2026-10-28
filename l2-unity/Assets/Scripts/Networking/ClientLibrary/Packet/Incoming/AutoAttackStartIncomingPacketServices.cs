using System;

/// <summary>
/// Auto-attack start wire packet handler entry (#142 batch slice 144).
/// </summary>
public static class AutoAttackStartIncomingPacketServices
{
    public static void ApplyAutoAttackStart(AutoAttackStartDto packet) =>
        WorldApplyPacketServices.ApplyWorld(apply => apply.AutoAttackStart(packet));
}
