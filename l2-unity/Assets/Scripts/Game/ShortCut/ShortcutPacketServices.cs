using VContainer;
/// <summary>
/// Game shortcut-layer access with [Inject] wiring and peer/live fallback (#142).
/// </summary>
public static class ShortcutPacketServices
{
    private static PlayerInventory _inventory;
    private static InputManager _input;
    private static PlayerActions _actions;
    private static PlayerShortcuts _shortcuts;

    public static void Bind(
        PlayerInventory inventory = null,
        InputManager input = null,
        PlayerActions actions = null,
        PlayerShortcuts shortcuts = null)
    {
        if (inventory != null)
            _inventory = inventory;
        if (input != null)
            _input = input;
        if (actions != null)
            _actions = actions;
        if (shortcuts != null)
            _shortcuts = shortcuts;
    }

    public static PlayerInventory Inventory =>
        _inventory != null ? _inventory : InventoryPacketServices.Inventory;

    public static InputManager Input =>
        _input != null ? _input : ControllerPacketServices.Input;

    public static PlayerActions Actions =>
        (_actions as UnityEngine.Object) != null
            ? _actions
            : ResolveFromGame<PlayerActions>() ?? LegacyPlayerActionsInstance();

    public static PlayerShortcuts Shortcuts =>
        _shortcuts != null ? _shortcuts : LegacyPlayerShortcutsInstance();

    static T ResolveFromGame<T>() where T : class
    {
        if (App.GameContainer == null)
            return null;

        try
        {
            return App.GameContainer.Resolve<T>();
        }
        catch
        {
            return null;
        }
    }

#pragma warning disable CS0618 // Locator fallback until all call sites are [Inject]
    static PlayerActions LegacyPlayerActionsInstance() => PlayerActions.Instance;
    static PlayerShortcuts LegacyPlayerShortcutsInstance() => PlayerShortcuts.Instance;
#pragma warning restore CS0618
}
