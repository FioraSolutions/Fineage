/// <summary>
/// Inventory UI access with [Inject] wiring and live fallback (#142).
/// </summary>
public static class InventoryPacketServices
{
    private static PlayerInventory _inventory;
    private static InventoryWindow _window;

    public static void Bind(PlayerInventory inventory = null, InventoryWindow window = null)
    {
        if (inventory != null)
            _inventory = inventory;
        if (window != null)
            _window = window;
    }

    public static PlayerInventory Inventory =>
        _inventory != null ? _inventory : PlayerInventory.Instance;

    public static InventoryWindow Window =>
        _window != null ? _window : InventoryWindow.Instance;
}
