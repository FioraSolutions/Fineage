using Fineage.Net.Contract;

public sealed partial class NetSession
{
    internal void ApplyRecipeBookList(RecipeBookListNetMsg msg) =>
        NetCraftApply.ApplyRecipeBookList(msg);

    internal void ApplyRecipeItemMakeInfo(RecipeItemMakeInfoNetMsg msg) =>
        NetCraftApply.ApplyRecipeItemMakeInfo(msg);

    internal void ApplyRecipeShopSellList(RecipeShopSellListNetMsg msg) =>
        NetCraftApply.ApplyRecipeShopSellList(msg);

    internal void ApplyRecipeShopItemInfo(RecipeShopItemInfoNetMsg msg) =>
        NetCraftApply.ApplyRecipeShopItemInfo(msg);

    internal void ApplyPrivateStoreInfo(PrivateStoreInfoNetMsg msg) =>
        NetPrivateStoreApply.ApplyPrivateStoreInfo(msg);
}