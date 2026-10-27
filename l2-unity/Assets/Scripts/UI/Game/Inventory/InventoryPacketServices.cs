/// <summary>
/// Inventory UI access to game services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 20).
/// </summary>
public static class InventoryPacketServices
{
    private static GameClient _game;
    private static PlayerInventory _inventory;

    public static void Bind(GameClient game = null, PlayerInventory inventory = null)
    {
        if (game != null)
            _game = game;
        if (inventory != null)
            _inventory = inventory;
    }

    public static GameClient Game =>
        _game != null ? _game : IncomingPacketActions.Game;

    public static PlayerInventory Inventory =>
        _inventory != null ? _inventory : IncomingPacketActions.Inventory;
}
