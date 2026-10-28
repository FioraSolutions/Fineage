using System;

/// <summary>
/// Crafting UI apply hub (#142 batch slice 161; locator slice 170).
/// </summary>
public static class CraftApplyIncomingPacketServices
{
    public static RecipeBookWindow RecipeBook => LocatorApplyIncomingPacketServices.RecipeBook;

    public static CraftingItemWindow Crafting => LocatorApplyIncomingPacketServices.Crafting;

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}
