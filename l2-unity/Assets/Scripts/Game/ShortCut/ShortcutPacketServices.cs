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
        _actions != null ? _actions : PlayerActions.Instance;

    public static PlayerShortcuts Shortcuts =>
        _shortcuts != null ? _shortcuts : PlayerShortcuts.Instance;
}
