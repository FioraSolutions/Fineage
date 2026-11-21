public static class ManufacturePacketServices
{
    private static RecipeShopManageWindow _manage;
    private static RecipeShopBrowseWindow _browse;

    public static void Bind(
        RecipeShopManageWindow manage = null,
        RecipeShopBrowseWindow browse = null)
    {
        if (manage != null)
            _manage = manage;
        if (browse != null)
            _browse = browse;
    }

    public static RecipeShopManageWindow Manage =>
        _manage != null ? _manage : RecipeShopManageWindow.Instance;

    public static RecipeShopBrowseWindow Browse =>
        _browse != null ? _browse : RecipeShopBrowseWindow.Instance;

    public static bool PickingRecipesForShop =>
        Manage != null && Manage.IsPickingRecipes;
}