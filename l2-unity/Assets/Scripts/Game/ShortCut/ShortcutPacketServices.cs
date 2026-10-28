/// <summary>
/// Game shortcut-layer access with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 30).
/// </summary>
public static class ShortcutPacketServices
{
    private static GameClient _game;
    private static PlayerInventory _inventory;
    private static InputManager _input;
    private static PlayerActions _actions;
    private static PlayerShortcuts _shortcuts;

    public static void Bind(
        GameClient game = null,
        PlayerInventory inventory = null,
        InputManager input = null,
        PlayerActions actions = null,
        PlayerShortcuts shortcuts = null)
    {
        if (game != null)
            _game = game;
        if (inventory != null)
            _inventory = inventory;
        if (input != null)
            _input = input;
        if (actions != null)
            _actions = actions;
        if (shortcuts != null)
            _shortcuts = shortcuts;
    }

    public static GameClient Game =>
        _game != null ? _game : IncomingPacketActions.Game;

    public static PlayerInventory Inventory =>
        _inventory != null ? _inventory : IncomingPacketActions.Inventory;

    public static InputManager Input =>
        _input != null ? _input : IncomingPacketActions.Input;

    public static PlayerActions Actions =>
        _actions != null ? _actions : IncomingPacketActions.Actions;

    public static PlayerShortcuts Shortcuts =>
        _shortcuts != null ? _shortcuts : IncomingPacketActions.Shortcuts;
}
