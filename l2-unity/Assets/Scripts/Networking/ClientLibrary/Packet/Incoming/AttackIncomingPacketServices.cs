using System;

/// <summary>
/// Attack wire packet handler entry (#142 batch slice 144).
/// Splits apply out of <see cref="CombatIncomingPacketServices"/> (sub-slice 48).
/// </summary>
public static class AttackIncomingPacketServices
{
    public static void ApplyAttack(AttackDto packet) =>
        WorldApplyPacketServices.ApplyWorld(apply => apply.Attack(packet));
}
