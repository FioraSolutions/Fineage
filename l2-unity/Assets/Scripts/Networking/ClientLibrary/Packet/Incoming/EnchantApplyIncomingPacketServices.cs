using System;

/// <summary>
/// Enchant UI apply hub (#142 batch slice 163; session hub slice 179).
/// </summary>
public static class EnchantApplyIncomingPacketServices
{
    public static EnchantWindow Enchant => LocatorApplyIncomingPacketServices.Enchant;

    public static void Queue(Action action) =>
        CombatPacketApplyIncomingPacketServices.Queue(action);
}
