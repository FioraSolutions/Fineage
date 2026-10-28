using System;

/// <summary>
/// Recipe item make info wire packet handler entry (#142 batch slice 140).
/// Apply facade (#142 batch slice 156). Re-homes from <see cref="RecipeItemMakeInfoIncomingPacketServices"/>.
/// </summary>
public static class RecipeItemMakeInfoApplyIncomingPacketServices
{
    public static void Queue(Action action) => CraftApplyIncomingPacketServices.Queue(action);

    public static void ApplyRecipeItemMakeInfo(RecipeItemMakeInfoDto packet) =>
        Queue(() =>
        {
            CraftApplyIncomingPacketServices.RecipeBook.HideWindow();
            CraftApplyIncomingPacketServices.Crafting.AddData(packet);
            CraftApplyIncomingPacketServices.Crafting.ShowWindow();
        });
}
