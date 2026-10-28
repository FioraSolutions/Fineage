using System;

/// <summary>
/// Recipe item make info wire packet handler entry (#142 batch slice 140).
/// Apply facade (#142 batch slice 156; locator queue slice 258). Re-homes from <see cref="RecipeItemMakeInfoIncomingPacketServices"/>.
/// </summary>
public static class RecipeItemMakeInfoApplyIncomingPacketServices
{
    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyRecipeItemMakeInfo(RecipeItemMakeInfoDto packet) =>
        Queue(() =>
        {
            LocatorApplyIncomingPacketServices.RecipeBook.HideWindow();
            LocatorApplyIncomingPacketServices.Crafting.AddData(packet);
            LocatorApplyIncomingPacketServices.Crafting.ShowWindow();
        });
}
