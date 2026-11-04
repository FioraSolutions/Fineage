/// <summary>
/// Craft UI access to game services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 10).
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
        _recipeBook != null ? _recipeBook : IncomingPacketActions.RecipeBook;

    public static CraftingItemWindow Crafting =>
        _crafting != null ? _crafting : IncomingPacketActions.Crafting;
}
