using System;

/// <summary>
/// Enchant UI apply hub (#142 batch slice 163; combat packet hub slice 167).
/// </summary>
public static class EnchantApplyIncomingPacketServices
{
    public static EnchantWindow Enchant => CombatPacketApplyIncomingPacketServices.Enchant;

    public static void Queue(Action action) =>
        CombatPacketApplyIncomingPacketServices.Queue(action);
}
