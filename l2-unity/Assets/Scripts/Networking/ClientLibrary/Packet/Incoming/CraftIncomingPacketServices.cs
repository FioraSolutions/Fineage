using System;

/// <summary>
/// Recipe book / crafting wire packet handler access via UiIncomingPacketServices (#142 sub-slice 71).
/// </summary>
public static class CraftIncomingPacketServices
{
    public static void Queue(Action action) => UiIncomingPacketServices.Queue(action);

    public static void ApplyRecipeBookItemList(RecipeBookItemListDto packet) =>
        Queue(() =>
        {
            UiIncomingPacketServices.RecipeBook.AddData(packet);
            UiIncomingPacketServices.RecipeBook.ShowWindow();
        });

    public static void ApplyRecipeItemMakeInfo(RecipeItemMakeInfoDto packet) =>
        Queue(() =>
        {
            UiIncomingPacketServices.RecipeBook.HideWindow();
            UiIncomingPacketServices.Crafting.AddData(packet);
            UiIncomingPacketServices.Crafting.ShowWindow();
        });
}
