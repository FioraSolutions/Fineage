using Fineage.Net.Contract;

/// <summary>Queues craft / manufacture Net packets into craft UI.</summary>
public static class NetCraftApply
{
    public static void ApplyRecipeBookList(RecipeBookListNetMsg msg)
    {
        RecipeBookItemListDto dto = NetCraftMappings.ToRecipeBookListDto(msg);
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            RecipeBookWindow book = CraftPacketServices.RecipeBook;
            if (book == null)
            {
                FineageLog.Error(FineageLogCategory.Net, "RecipeBookList: RecipeBookWindow is not bound.");
                return;
            }

            book.AddData(dto);
            book.ShowWindow();
        });
    }

    public static void ApplyRecipeItemMakeInfo(RecipeItemMakeInfoNetMsg msg)
    {
        if (msg == null)
            return;

        NetCraftSession.LastCurMp = msg.CurrentMp;
        NetCraftSession.LastMaxMp = msg.MaxMp;

        bool afterMakeSelf = NetCraftSession.PendingMakeSelfRecipeId == msg.RecipeId;
        if (afterMakeSelf)
        {
            NetCraftSession.PendingMakeSelfRecipeId = 0;
            NetCraftFeedback.NoteMakeResult(msg, shop: false);
        }

        RecipeItemMakeInfoDto dto = NetCraftMappings.ToMakeInfoDto(msg);
        if (dto == null)
            return;

        QueueApplyIncomingPacketServices.Queue(() =>
        {
            CraftingItemWindow craft = CraftPacketServices.Crafting;
            if (craft == null)
            {
                FineageLog.Error(FineageLogCategory.Net, "RecipeItemMakeInfo: CraftingItemWindow is not bound.");
                return;
            }

            craft.AddData(dto);
            craft.ShowWindow();
        });
    }

    public static void ApplyRecipeShopSellList(RecipeShopSellListNetMsg msg)
    {
        var offers = NetCraftMappings.ToShopOffers(msg);
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            RecipeShopBrowseWindow browse = ManufacturePacketServices.Browse;
            if (browse == null)
            {
                FineageLog.Error(FineageLogCategory.Net, "RecipeShopSellList: RecipeShopBrowseWindow is not bound.");
                return;
            }

            browse.OpenList(msg.ManufacturerObjectId, msg.CurrentMp, msg.MaxMp, msg.BuyerAdena, offers);
        });
    }

    public static void ApplyRecipeShopItemInfo(RecipeShopItemInfoNetMsg msg)
    {
        if (msg.Result == 0 || msg.Result == 1)
            NetCraftFeedback.NoteShopMakeResult(msg);

        RecipeItemMakeInfoDto dto = NetCraftMappings.ToShopMakeInfoDto(msg);
        if (dto == null)
            return;

        QueueApplyIncomingPacketServices.Queue(() =>
        {
            CraftingItemWindow craft = CraftPacketServices.Crafting;
            if (craft == null)
            {
                FineageLog.Error(FineageLogCategory.Net, "RecipeShopItemInfo: CraftingItemWindow is not bound.");
                return;
            }

            craft.AddData(dto);
            craft.ShowWindow();
        });
    }
}