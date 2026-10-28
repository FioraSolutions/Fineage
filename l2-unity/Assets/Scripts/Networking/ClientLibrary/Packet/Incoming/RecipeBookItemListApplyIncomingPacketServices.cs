using System;

/// <summary>
/// Recipe book list wire packet handler entry (#142 batch slice 140).
/// Apply facade (#142 batch slice 156). Re-homes from <see cref="RecipeBookItemListIncomingPacketServices"/>.
/// </summary>
public static class RecipeBookItemListApplyIncomingPacketServices
{
    public static void Queue(Action action) => UiIncomingPacketServices.Queue(action);

    public static void ApplyRecipeBookItemList(RecipeBookItemListDto packet) =>
        Queue(() =>
        {
            UiIncomingPacketServices.RecipeBook.AddData(packet);
            UiIncomingPacketServices.RecipeBook.ShowWindow();
        });
}
