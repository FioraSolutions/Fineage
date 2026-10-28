using System;

/// <summary>
/// Crafting UI apply hub (#142 batch slice 161; UI hub slice 167).
/// </summary>
public static class CraftApplyIncomingPacketServices
{
    public static RecipeBookWindow RecipeBook => UiApplyIncomingPacketServices.RecipeBook;

    public static CraftingItemWindow Crafting => UiApplyIncomingPacketServices.Crafting;

    public static void Queue(Action action) => UiApplyIncomingPacketServices.Queue(action);
}
