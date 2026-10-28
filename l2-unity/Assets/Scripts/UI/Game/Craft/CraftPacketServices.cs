/// <summary>
/// Craft UI access to game services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 10).
/// </summary>
public static class CraftPacketServices
{
    private static GameClient _game;
    private static RecipeBookWindow _recipeBook;
    private static CraftingItemWindow _crafting;

    public static void Bind(
        GameClient game = null,
        RecipeBookWindow recipeBook = null,
        CraftingItemWindow crafting = null)
    {
        if (game != null)
            _game = game;
        if (recipeBook != null)
            _recipeBook = recipeBook;
        if (crafting != null)
            _crafting = crafting;
    }

    public static GameClient Game =>
        _game != null ? _game : IncomingPacketActions.Game;

    public static RecipeBookWindow RecipeBook =>
        _recipeBook != null ? _recipeBook : IncomingPacketActions.RecipeBook;

    public static CraftingItemWindow Crafting =>
        _crafting != null ? _crafting : IncomingPacketActions.Crafting;
}
