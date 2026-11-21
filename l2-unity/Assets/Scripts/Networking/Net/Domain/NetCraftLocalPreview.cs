using Fineage.Net.Contract;

/// <summary>Self-craft detail window: materials come from recipe datapack; MP from last zone stats.</summary>
public static class NetCraftLocalPreview
{
    public static void OpenForRecipe(int recipeId)
    {
        RecipeData data = TablePacketServices.Recipes.GetRecipeData(recipeId);
        if (data == null)
        {
            FineageLog.Error(FineageLogCategory.Net, $"Craft preview: missing RecipeData id_mk={recipeId}");
            return;
        }

        var msg = new RecipeItemMakeInfoNetMsg
        {
            RecipeId = recipeId,
            Result = 2,
            CurrentMp = NetCraftSession.LastCurMp,
            MaxMp = NetCraftSession.LastMaxMp
        };
        RecipeItemMakeInfoDto dto = NetCraftMappings.ToMakeInfoDto(msg);
        if (dto == null)
            return;

        CraftingItemWindow craft = CraftPacketServices.Crafting;
        if (craft == null)
        {
            FineageLog.Error(FineageLogCategory.Net, "Craft preview: CraftingItemWindow is not bound.");
            return;
        }

        craft.AddData(dto);
        craft.ShowWindow();
    }
}