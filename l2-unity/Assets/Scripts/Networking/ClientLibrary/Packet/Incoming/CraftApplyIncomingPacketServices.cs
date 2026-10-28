using System;

/// <summary>
/// Crafting UI apply hub (#142 batch slice 161; locator slice 170).
/// </summary>
public static class CraftApplyIncomingPacketServices
{
    public static RecipeBookWindow RecipeBook => IncomingPacketLocatorApplyIncomingPacketServices.RecipeBook;

    public static CraftingItemWindow Crafting => IncomingPacketLocatorApplyIncomingPacketServices.Crafting;

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}
