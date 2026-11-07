using System;

/// <summary>
/// Combat die apply, constructed by the container instead of reached through the
/// static <c>*ApplyIncomingPacketServices</c> relay (#142 slice 3 pilot).
/// </summary>
public sealed class CombatDieApplyService
{
    public void Apply(DieDto packet) =>
        QueueApplyIncomingPacketServices.ApplyWorld(apply => apply.Die(packet));
}
