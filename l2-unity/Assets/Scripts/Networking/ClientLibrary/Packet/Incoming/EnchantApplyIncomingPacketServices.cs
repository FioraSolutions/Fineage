using System;

/// <summary>
/// Enchant UI apply hub (#142 batch slice 163; locator slice 171).
/// </summary>
public static class EnchantApplyIncomingPacketServices
{
    public static EnchantWindow Enchant => LocatorApplyIncomingPacketServices.Enchant;

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}
