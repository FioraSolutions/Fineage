using Fineage.Net.Contract;

/// <summary>Local craft result lines when the zone only echoes RecipeItemMakeInfo result.</summary>
public static class NetCraftFeedback
{
    public static void NoteMakeResult(RecipeItemMakeInfoNetMsg msg, bool shop)
    {
        if (msg == null)
            return;
        if (msg.Result != 0 && msg.Result != 1)
            return;

        RecipeData data = TablePacketServices.Recipes.GetRecipeData(msg.RecipeId);
        if (data == null)
        {
            FineageLog.Error(
                FineageLogCategory.Net,
                $"Craft result feedback: missing RecipeData id_mk={msg.RecipeId}");
            return;
        }

        ItemName itemName = TablePacketServices.ItemNames.GetItemName(data.ItemId);
        string product = itemName != null ? itemName.DisplayName : $"item:{data.ItemId}";
        string line = msg.Result == 0
            ? (shop ? $"Craft purchase succeeded ({product})." : $"You succeeded in crafting {product}.")
            : (shop ? $"Craft purchase failed ({product})." : $"You failed to craft {product}.");
        NetChatFeed.NoteSystem(line);
    }

    public static void NoteShopMakeResult(RecipeShopItemInfoNetMsg msg)
    {
        if (msg == null)
            return;
        var pseudo = new RecipeItemMakeInfoNetMsg
        {
            RecipeId = msg.RecipeId,
            Result = msg.Result,
            CurrentMp = msg.CurrentMp,
            MaxMp = msg.MaxMp
        };
        NoteMakeResult(pseudo, shop: true);
    }
}