using System;

/// <summary>
/// Recipe book / crafting apply hub (#142 batch slice 161).
/// </summary>
public static class CraftApplyIncomingPacketServices
{
    public static RecipeBookWindow RecipeBook => IncomingPacketLocatorApplyIncomingPacketServices.RecipeBook;

    public static CraftingItemWindow Crafting => IncomingPacketLocatorApplyIncomingPacketServices.Crafting;

    public static void Queue(Action action) => IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}
