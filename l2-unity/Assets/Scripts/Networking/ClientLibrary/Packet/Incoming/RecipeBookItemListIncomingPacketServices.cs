using System;

/// <summary>
/// Recipe book list wire packet handler entry (#142 batch slice 140).
/// </summary>
public static class RecipeBookItemListIncomingPacketServices
{
    public static void Queue(Action action) => UiIncomingPacketServices.Queue(action);

    public static void ApplyRecipeBookItemList(RecipeBookItemListDto packet) =>
        Queue(() =>
        {
            UiIncomingPacketServices.RecipeBook.AddData(packet);
            UiIncomingPacketServices.RecipeBook.ShowWindow();
        });
}
