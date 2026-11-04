using System;

/// <summary>
/// Combat attack apply, constructed by the container instead of reached through the
/// static <c>*ApplyIncomingPacketServices</c> relay (#142 slice 3 pilot).
/// </summary>
public sealed class CombatAttackApplyService
{
    public void Apply(AttackDto packet) =>
        QueueApplyIncomingPacketServices.ApplyWorld(apply => apply.Attack(packet));
}
