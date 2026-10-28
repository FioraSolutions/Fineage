using System;

/// <summary>
/// Enchant UI apply hub (#142 batch slice 163; combat world slice 175).
/// </summary>
public static class EnchantApplyIncomingPacketServices
{
    public static EnchantWindow Enchant => LocatorApplyIncomingPacketServices.Enchant;

    public static void Queue(Action action) =>
        CombatWorldApplyIncomingPacketServices.Queue(action);
}
