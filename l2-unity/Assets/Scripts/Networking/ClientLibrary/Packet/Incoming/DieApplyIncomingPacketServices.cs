using System;

/// <summary>
/// Die wire packet handler entry (#142 batch slice 144).
/// Apply facade (#142 batch slice 153). Re-homes from <see cref="DieIncomingPacketServices"/>.
/// </summary>
public static class DieApplyIncomingPacketServices
{
    public static void ApplyDie(DieDto packet) =>
        CombatSessionApplyIncomingPacketServices.ApplyWorld(apply => apply.Die(packet));
}
