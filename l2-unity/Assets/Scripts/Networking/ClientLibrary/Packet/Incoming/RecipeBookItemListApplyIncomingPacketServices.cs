using System;

/// <summary>
/// Recipe book list wire packet handler entry (#142 batch slice 140).
/// Apply facade (#142 batch slice 156; locator queue slice 258). Re-homes from <see cref="RecipeBookItemListIncomingPacketServices"/>.
/// </summary>
public static class RecipeBookItemListApplyIncomingPacketServices
{
    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyRecipeBookItemList(RecipeBookItemListDto packet) =>
        Queue(() =>
        {
            LocatorApplyIncomingPacketServices.RecipeBook.AddData(packet);
            LocatorApplyIncomingPacketServices.RecipeBook.ShowWindow();
        });
}
