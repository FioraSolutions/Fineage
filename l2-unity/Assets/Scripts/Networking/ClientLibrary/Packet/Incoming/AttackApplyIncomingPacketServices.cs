using System;

/// <summary>
/// Attack wire packet handler entry (#142 batch slice 144).
/// Splits apply out of <see cref="CombatApplyIncomingPacketServices"/> (sub-slice 48).
/// Apply facade (#142 batch slice 153). Re-homes from <see cref="AttackIncomingPacketServices"/>.
/// </summary>
public static class AttackApplyIncomingPacketServices
{
    public static void ApplyAttack(AttackDto packet) =>
        CombatApplyIncomingPacketServices.ApplyWorld(apply => apply.Attack(packet));
}
