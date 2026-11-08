/// <summary>
/// Craft UI access to game services with [Inject] wiring and live fallback (#142).
/// </summary>
public static class CraftPacketServices
{
    private static RecipeBookWindow _recipeBook;
    private static CraftingItemWindow _crafting;

    public static void Bind(
        RecipeBookWindow recipeBook = null,
        CraftingItemWindow crafting = null)
    {
        if (recipeBook != null)
            _recipeBook = recipeBook;
        if (crafting != null)
            _crafting = crafting;
    }

    public static RecipeBookWindow RecipeBook =>
        _recipeBook != null ? _recipeBook : RecipeBookWindow.Instance;

    public static CraftingItemWindow Crafting =>
        _crafting != null ? _crafting : CraftingItemWindow.Instance;
}
